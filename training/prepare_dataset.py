"""
Merges raw, per-dataset image folders into a single train/valid folder tree
keyed by canonical label, ready to feed into LeafUpload.ModelExporter for
retraining (see training/label_map.json for the label taxonomy and per-source
mapping config).

Usage:
    # 1. Inspect a freshly downloaded dataset to see what its leaf folders are called
    python prepare_dataset.py discover --source "C:\\path\\to\\PlantDoc"

    # 2. Fill in training/label_map.json with the mapping, then merge everything
    python prepare_dataset.py merge --config label_map.json --out merged_dataset --val-split 0.1
"""
import argparse
import hashlib
import json
import random
import shutil
import sys
from pathlib import Path

IMAGE_EXTENSIONS = {".png", ".jpg", ".jpeg", ".bmp"}

try:
    from PIL import Image
except ImportError:
    Image = None


def is_valid_image(path: Path) -> bool:
    if Image is None:
        return True
    try:
        with Image.open(path) as img:
            img.verify()
        return True
    except Exception:
        return False


def find_leaf_image_dirs(root: Path):
    """Yield every directory that directly contains at least one image file."""
    for dirpath in sorted(p for p in root.rglob("*") if p.is_dir()) + [root]:
        images = [f for f in dirpath.iterdir() if f.is_file() and f.suffix.lower() in IMAGE_EXTENSIONS]
        if images:
            yield dirpath, images


def cmd_discover(args):
    root = Path(args.source)
    if not root.is_dir():
        sys.exit(f"Not a directory: {root}")

    found = list(find_leaf_image_dirs(root))
    if not found:
        print("No image-containing folders found.")
        return

    print(f"Found {len(found)} leaf folder(s) under {root}:\n")
    for dirpath, images in found:
        print(f"  {dirpath.name!r:55s} ({len(images)} images)  [{dirpath}]")
    print("\nCopy the folder names above into training/label_map.json's \"mapping\" for this source.")


def copy_file(src: Path, dst: Path, link: bool):
    dst.parent.mkdir(parents=True, exist_ok=True)
    if link:
        try:
            import os
            os.link(src, dst)
            return
        except OSError:
            pass  # cross-volume or unsupported - fall back to copy
    shutil.copy2(src, dst)


def split_files(files, val_split, seed):
    files = list(files)
    random.Random(seed).shuffle(files)
    n_val = max(1, int(len(files) * val_split)) if len(files) > 1 else 0
    return files[n_val:], files[:n_val]


def cmd_merge(args):
    config = json.loads(Path(args.config).read_text(encoding="utf-8"))
    canonical_labels = set(config["canonical_labels"])
    out_root = Path(args.out)
    train_root = out_root / "train"
    valid_root = out_root / "valid"
    unmapped_root = out_root / "_unmapped"

    counts = {"train": {}, "valid": {}}
    seen_hashes = {}  # canonical_label -> set of md5 hashes, only if --dedupe
    skipped_corrupt = 0
    unmapped_folders = []

    def add_image(label, split, src_path, source_name):
        nonlocal skipped_corrupt
        if not is_valid_image(src_path):
            skipped_corrupt += 1
            return
        if args.dedupe:
            digest = hashlib.md5(src_path.read_bytes()).hexdigest()
            bucket = seen_hashes.setdefault(label, set())
            if digest in bucket:
                return
            bucket.add(digest)
        dest_dir = (train_root if split == "train" else valid_root) / label
        dest_name = f"{source_name}_{src_path.name}"
        copy_file(src_path, dest_dir / dest_name, args.link)
        counts[split][label] = counts[split].get(label, 0) + 1

    for source in config["sources"]:
        name = source["name"]
        if name.startswith("EXAMPLE_"):
            continue
        print(f"\n=== Source: {name} ===")

        if source.get("already_split"):
            mapping = source.get("mapping", {})
            for split, key in (("train", "train_path"), ("valid", "valid_path")):
                split_root = Path(source[key])
                if not split_root.is_dir():
                    print(f"  WARNING: {key} not found: {split_root} (skipping)")
                    continue
                for label_dir in sorted(d for d in split_root.iterdir() if d.is_dir()):
                    label = mapping.get(label_dir.name, label_dir.name)
                    if label not in canonical_labels:
                        unmapped_folders.append(label_dir.name)
                        images = [f for f in label_dir.iterdir() if f.is_file() and f.suffix.lower() in IMAGE_EXTENSIONS]
                        for img in images:
                            copy_file(img, unmapped_root / label_dir.name / img.name, args.link)
                        print(f"  WARNING: folder '{label_dir.name}' has no mapping to a canonical label - copied to _unmapped instead")
                        continue
                    images = [f for f in label_dir.iterdir() if f.is_file() and f.suffix.lower() in IMAGE_EXTENSIONS]
                    for img in images:
                        add_image(label, split, img, name)
                    print(f"  {split}/{label}: {len(images)} images")
        else:
            root = Path(source["path"])
            mapping = source.get("mapping", {})
            if not root.is_dir():
                print(f"  WARNING: path not found: {root} (skipping)")
                continue
            for dirpath, images in find_leaf_image_dirs(root):
                label = mapping.get(dirpath.name)
                if label is None:
                    unmapped_folders.append(dirpath.name)
                    for img in images:
                        copy_file(img, unmapped_root / dirpath.name / img.name, args.link)
                    continue
                if label not in canonical_labels:
                    print(f"  NOTE: '{label}' is a new canonical label not yet in label_map.json's canonical_labels list")
                train_files, valid_files = split_files(images, args.val_split, args.seed)
                for img in train_files:
                    add_image(label, "train", img, name)
                for img in valid_files:
                    add_image(label, "valid", img, name)
                print(f"  {dirpath.name!r} -> {label}: {len(train_files)} train / {len(valid_files)} valid")

    print("\n=== Merge summary ===")
    all_labels = sorted(set(counts["train"]) | set(counts["valid"]))
    for label in all_labels:
        t, v = counts["train"].get(label, 0), counts["valid"].get(label, 0)
        flag = "  <-- sparse, consider adding more images" if (t + v) < args.sparse_threshold else ""
        print(f"  {label:65s} train={t:<6} valid={v:<6}{flag}")

    if skipped_corrupt:
        print(f"\nSkipped {skipped_corrupt} unreadable/corrupt image(s).")
    if unmapped_folders:
        uniq = sorted(set(unmapped_folders))
        print(f"\n{len(uniq)} unmapped source folder(s) copied to {unmapped_root} for review:")
        for u in uniq:
            print(f"  - {u}")
        print("Add these to the relevant source's \"mapping\" in label_map.json and re-run to include them.")

    print(f"\nMerged dataset written to: {out_root.resolve()}")


def main():
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    sub = parser.add_subparsers(dest="command", required=True)

    p_discover = sub.add_parser("discover", help="List leaf image folders in a raw dataset")
    p_discover.add_argument("--source", required=True)
    p_discover.set_defaults(func=cmd_discover)

    p_merge = sub.add_parser("merge", help="Merge configured sources into train/valid folders")
    p_merge.add_argument("--config", default="label_map.json")
    p_merge.add_argument("--out", default="merged_dataset")
    p_merge.add_argument("--val-split", type=float, default=0.1, help="Fraction held out for validation (sources without a pre-existing split)")
    p_merge.add_argument("--seed", type=int, default=42)
    p_merge.add_argument("--dedupe", action="store_true", help="Skip duplicate images (by content hash) within a label")
    p_merge.add_argument("--link", action="store_true", help="Hard-link instead of copy where possible (saves disk space)")
    p_merge.add_argument("--sparse-threshold", type=int, default=50, help="Flag labels with fewer than this many total images")
    p_merge.set_defaults(func=cmd_merge)

    args = parser.parse_args()
    args.func(args)


if __name__ == "__main__":
    main()

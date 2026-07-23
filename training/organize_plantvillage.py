"""
Extracts the official PlantVillage release (huggingface.co/datasets/mohanty/PlantVillage)
into plain train/<Label>/*.jpg and valid/<Label>/*.jpg folders, using the
dataset's own leak-safe split lists (splits/color_train.txt, splits/color_test.txt)
so images from the same physical leaf don't end up split across train and valid.

Usage:
    python organize_plantvillage.py --zip data.zip --train-list color_train.txt --test-list color_test.txt --out organized
"""
import argparse
import zipfile
from pathlib import Path


def organize(zip_path: Path, train_list: Path, test_list: Path, out_root: Path):
    splits = {
        "train": [line.strip() for line in train_list.read_text(encoding="utf-8").splitlines() if line.strip()],
        "valid": [line.strip() for line in test_list.read_text(encoding="utf-8").splitlines() if line.strip()],
    }

    with zipfile.ZipFile(zip_path) as z:
        available = set(z.namelist())
        for split, entries in splits.items():
            print(f"\n=== {split}: {len(entries)} listed images ===")
            missing = 0
            written = 0
            for i, rel_path in enumerate(entries, 1):
                if rel_path not in available:
                    missing += 1
                    continue
                # rel_path looks like "raw/color/Apple___Apple_scab/xxx.JPG"
                parts = rel_path.split("/")
                label, filename = parts[-2], parts[-1]
                dest = out_root / split / label / filename
                dest.parent.mkdir(parents=True, exist_ok=True)
                with z.open(rel_path) as src, open(dest, "wb") as dst:
                    dst.write(src.read())
                written += 1
                if i % 5000 == 0:
                    print(f"  {i}/{len(entries)} done...")
            print(f"  wrote {written} images, {missing} listed paths not found in zip")


def main():
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("--zip", required=True)
    parser.add_argument("--train-list", required=True)
    parser.add_argument("--test-list", required=True)
    parser.add_argument("--out", default="organized")
    args = parser.parse_args()

    organize(Path(args.zip), Path(args.train_list), Path(args.test_list), Path(args.out))
    print(f"\nDone. Organized dataset at: {Path(args.out).resolve()}")


if __name__ == "__main__":
    main()

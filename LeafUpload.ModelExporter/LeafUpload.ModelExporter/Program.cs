using System.Diagnostics;
using LeafUpload_Infrastructure;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Vision;

var options = CliOptions.Parse(args);
if (options == null)
    return 1;

var trainFolder = Path.Combine(options.DataRoot, "train");
var validFolder = Path.Combine(options.DataRoot, "valid");
if (!Directory.Exists(trainFolder) || !Directory.Exists(validFolder))
{
    Console.Error.WriteLine($"Expected '{trainFolder}' and '{validFolder}' (run training/prepare_dataset.py first).");
    return 1;
}

var mlContext = new MLContext(seed: 1);

Console.WriteLine("Loading training/validation file lists (image bytes are streamed lazily, not preloaded)...");
var trainData = mlContext.Data.LoadFromEnumerable(EnumerateImages(trainFolder, options.MaxImagesPerClass));
var validData = mlContext.Data.LoadFromEnumerable(EnumerateImages(validFolder, options.MaxImagesPerClass));

// Key the Label column once and reuse the same fitted transform for both
// train and validation data, so both share an identical label vocabulary.
var mapLabelToKey = mlContext.Transforms.Conversion.MapValueToKey("Label", "Label");
var mapLabelToKeyTransformer = mapLabelToKey.Fit(trainData);
var trainDataKeyed = mapLabelToKeyTransformer.Transform(trainData);
var validDataKeyed = mapLabelToKeyTransformer.Transform(validData);

var trainerOptions = new ImageClassificationTrainer.Options
{
    FeatureColumnName = "ImageSource",
    LabelColumnName = "Label",
    Arch = options.Arch,
    Epoch = options.Epochs,
    BatchSize = options.BatchSize,
    LearningRate = options.LearningRate,
    ValidationSet = validDataKeyed,
    WorkspacePath = options.WorkspacePath,
    ReuseTrainSetBottleneckCachedValues = true,
    ReuseValidationSetBottleneckCachedValues = true,
    TestOnTrainSet = false,
    MetricsCallback = m => Console.WriteLine(m),
};

Console.WriteLine($"Training: arch={options.Arch}, epochs={options.Epochs}, batchSize={options.BatchSize}, workspace={options.WorkspacePath}");
var stopwatch = Stopwatch.StartNew();

var trainingPipeline = mlContext.MulticlassClassification.Trainers.ImageClassification(trainerOptions);
var trainedTransformer = trainingPipeline.Fit(trainDataKeyed);

Console.WriteLine($"Training finished in {stopwatch.Elapsed}. Evaluating on validation set...");

// Evaluate before mapping the key-typed PredictedLabel back to text, since
// Evaluate needs Label/PredictedLabel to be the same key type for the confusion matrix.
var rawPredictions = trainedTransformer.Transform(validDataKeyed);
var metrics = mlContext.MulticlassClassification.Evaluate(rawPredictions, labelColumnName: "Label");
PrintMetrics(metrics, mapLabelToKeyTransformer, mlContext, validDataKeyed);

// Build the final exportable model by composing the already-trained transformer
// with the label<->key mappings, instead of re-fitting (which would retrain the
// whole classifier a second time just to get human-readable labels back).
var mapKeyToValue = mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel", "PredictedLabel");
var mapKeyToValueTransformer = mapKeyToValue.Fit(rawPredictions);
var exportModel = new TransformerChain<ITransformer>(mapLabelToKeyTransformer, trainedTransformer, mapKeyToValueTransformer);

Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(options.OutputPath))!);
mlContext.Model.Save(exportModel, trainData.Schema, options.OutputPath);
Console.WriteLine($"Saved retrained model to {options.OutputPath}");
Console.WriteLine("Swap it into LeafUpload.Infrastructure/MLModel1.mlnet once you're happy with the metrics above, then rebuild LeafUpload.Web.");

return 0;

static IEnumerable<MLModel1.ModelInput> EnumerateImages(string folder, int? maxPerClass)
{
    var allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".bmp" };
    var root = new DirectoryInfo(folder);
    foreach (var classDir in root.GetDirectories().OrderBy(d => d.Name))
    {
        var count = 0;
        foreach (var file in classDir.EnumerateFiles().OrderBy(f => f.Name))
        {
            if (!allowedExtensions.Contains(file.Extension.ToLowerInvariant()))
                continue;
            if (maxPerClass.HasValue && count >= maxPerClass.Value)
                break;
            count++;

            yield return new MLModel1.ModelInput
            {
                Label = classDir.Name,
                ImageSource = File.ReadAllBytes(file.FullName),
            };
        }
    }
}

static void PrintMetrics(MulticlassClassificationMetrics metrics, ITransformer mapLabelToKeyTransformer, MLContext mlContext, IDataView validDataKeyed)
{
    Console.WriteLine();
    Console.WriteLine($"MicroAccuracy:    {metrics.MicroAccuracy:P2}");
    Console.WriteLine($"MacroAccuracy:    {metrics.MacroAccuracy:P2}");
    Console.WriteLine($"LogLoss:          {metrics.LogLoss:F4}");
    Console.WriteLine();

    var labelColumn = validDataKeyed.Schema["Label"];
    VBuffer<ReadOnlyMemory<char>> keyValues = default;
    labelColumn.GetKeyValues(ref keyValues);
    var labelNames = keyValues.DenseValues().Select(v => v.ToString()).ToArray();

    Console.WriteLine("Per-class log-loss (higher = model struggles more on that class):");
    for (var i = 0; i < metrics.PerClassLogLoss.Count && i < labelNames.Length; i++)
    {
        Console.WriteLine($"  {labelNames[i],-65} {metrics.PerClassLogLoss[i]:F4}");
    }
}

internal sealed class CliOptions
{
    public required string DataRoot { get; init; }
    public string OutputPath { get; init; } = "RetrainedModel.mlnet";
    public int Epochs { get; init; } = 200;
    public int BatchSize { get; init; } = 10;
    public float LearningRate { get; init; } = 0.01f;
    public string WorkspacePath { get; init; } = "workspace";
    public int? MaxImagesPerClass { get; init; }
    public ImageClassificationTrainer.Architecture Arch { get; init; } = ImageClassificationTrainer.Architecture.ResnetV250;

    public static CliOptions? Parse(string[] args)
    {
        string? dataRoot = null;
        var outputPath = "RetrainedModel.mlnet";
        var epochs = 200;
        var batchSize = 10;
        var learningRate = 0.01f;
        var workspacePath = "workspace";
        int? maxImagesPerClass = null;
        var arch = ImageClassificationTrainer.Architecture.ResnetV250;

        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--data":
                    dataRoot = args[++i];
                    break;
                case "--out":
                    outputPath = args[++i];
                    break;
                case "--epochs":
                    epochs = int.Parse(args[++i]);
                    break;
                case "--batch-size":
                    batchSize = int.Parse(args[++i]);
                    break;
                case "--learning-rate":
                    learningRate = float.Parse(args[++i]);
                    break;
                case "--workspace":
                    workspacePath = args[++i];
                    break;
                case "--max-images-per-class":
                    maxImagesPerClass = int.Parse(args[++i]);
                    break;
                case "--arch":
                    arch = Enum.Parse<ImageClassificationTrainer.Architecture>(args[++i], ignoreCase: true);
                    break;
                case "-h":
                case "--help":
                    PrintUsage();
                    return null;
                default:
                    Console.Error.WriteLine($"Unknown argument: {args[i]}");
                    PrintUsage();
                    return null;
            }
        }

        if (dataRoot == null)
        {
            Console.Error.WriteLine("Missing required --data <mergedDatasetRoot>");
            PrintUsage();
            return null;
        }

        return new CliOptions
        {
            DataRoot = dataRoot,
            OutputPath = outputPath,
            Epochs = epochs,
            BatchSize = batchSize,
            LearningRate = learningRate,
            WorkspacePath = workspacePath,
            MaxImagesPerClass = maxImagesPerClass,
            Arch = arch,
        };
    }

    private static void PrintUsage()
    {
        Console.WriteLine("""
            Retrains the leaf disease model from a merged dataset (see training/prepare_dataset.py).

            Usage:
              dotnet run -- --data <mergedDatasetRoot> [options]

            Required:
              --data <path>                Folder containing train/ and valid/ subfolders (one per label)

            Options:
              --out <path>                 Output .mlnet path (default: RetrainedModel.mlnet)
              --epochs <n>                 Training epochs (default: 200)
              --batch-size <n>             Batch size (default: 10)
              --learning-rate <f>          Learning rate (default: 0.01)
              --workspace <path>           Bottleneck cache folder, speeds up re-runs (default: workspace)
              --max-images-per-class <n>   Cap images per class - use a small number for a fast smoke test
              --arch <name>                ImageClassificationTrainer architecture (default: ResnetV250)
            """);
    }
}

using UnityEngine;

[RequireComponent(typeof(Terrain))]
public class TerrainLayers : MonoBehaviour
{
    [Header("Layer Settings")]
    public TerrainLayer snowLayer;
    public TerrainLayer mountainLayer;

    [Header("Fractal Noise Settings")]
    [Range(0.001f, 0.1f)] public float frequency = 0.01f;
    [Range(1, 10)] public int octaves = 6;
    [Range(1.1f, 3.0f)] public float lacunarity = 2.0f;
    [Range(0.1f, 0.9f)] public float gain = 0.5f;
    public Vector2 offset;

    [Header("Coverage Targets")]
    [Range(0.7f, 0.9f)] public float snowTarget = 0.8f;
    [Range(0.1f, 0.3f)] public float mountainTarget = 0.2f;

    [Header("Mountain Intensity")]
    [Range(0.1f, 1f)] public float minMountainOpacity = 0.3f;
    [Range(0.5f, 1f)] public float maxMountainOpacity = 0.9f;
    [Range(0.1f, 1f)] public float intensityVariation = 0.7f;

    private Terrain terrain;
    private TerrainData terrainData;
    private System.Random random;

    private void Start()
    {
        terrain = GetComponent<Terrain>();
        terrainData = terrain.terrainData;
        random = new System.Random(Time.time.GetHashCode());

        GenerateTerrainLayers();
    }

    private void GenerateTerrainLayers()
    {
        if (snowLayer == null || mountainLayer == null)
        {
            Debug.LogError("Snow or Mountain layer not assigned!");
            return;
        }

        terrainData.terrainLayers = new TerrainLayer[] { snowLayer, mountainLayer };

        int width = terrainData.alphamapWidth;
        int height = terrainData.alphamapHeight;
        float[,,] alphaMap = new float[width, height, 2];

        // Generate primary fractal noise for mountain placement
        float[,] mountainNoise = GenerateFractalNoise(width, height);
        float mountainThreshold = FindThresholdForCoverage(mountainNoise, mountainTarget);

        // Generate secondary noise for intensity variation
        float[,] intensityNoise = GenerateFractalNoise(width, height, 0.5f);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float mountainValue = mountainNoise[x, y];

                if (mountainValue > mountainThreshold)
                {
                    // Calculate intensity with variation
                    float intensity = Mathf.Lerp(minMountainOpacity, maxMountainOpacity,
                        intensityNoise[x, y] * intensityVariation +
                        (float)(random.NextDouble() * 0.2f - 0.1f));

                    alphaMap[x, y, 1] = intensity; // Varying mountain intensity
                    alphaMap[x, y, 0] = 1f - intensity; // Snow shows through based on mountain opacity
                }
                else
                {
                    alphaMap[x, y, 0] = 1f; // Full snow
                    alphaMap[x, y, 1] = 0f; // No mountain
                }
            }
        }

        terrainData.SetAlphamaps(0, 0, alphaMap);
    }

    private float[,] GenerateFractalNoise(int width, int height, float frequencyMultiplier = 1f)
    {
        float[,] noiseMap = new float[width, height];

        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = random.Next(-10000, 10000) + offset.x;
            float offsetY = random.Next(-10000, 10000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;
        float halfWidth = width * 0.5f;
        float halfHeight = height * 0.5f;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float amplitude = 1;
                float currentFrequency = frequency * frequencyMultiplier;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth) * currentFrequency + octaveOffsets[i].x;
                    float sampleY = (y - halfHeight) * currentFrequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= gain;
                    currentFrequency *= lacunarity;
                }

                noiseMap[x, y] = noiseHeight;

                if (noiseHeight > maxNoiseHeight) maxNoiseHeight = noiseHeight;
                if (noiseHeight < minNoiseHeight) minNoiseHeight = noiseHeight;
            }
        }

        // Normalize
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }

    private float FindThresholdForCoverage(float[,] noiseMap, float targetCoverage)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        int totalPixels = width * height;
        int targetPixels = Mathf.FloorToInt(totalPixels * targetCoverage);

        float[] values = new float[totalPixels];
        for (int y = 0, i = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++, i++)
            {
                values[i] = noiseMap[x, y];
            }
        }

        System.Array.Sort(values);
        return values[totalPixels - targetPixels];
    }
}

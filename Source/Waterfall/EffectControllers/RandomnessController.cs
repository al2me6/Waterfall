﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Waterfall
{
  /// <summary>
  ///   A controller that generates randomness
  /// </summary>
  [Serializable]
  [DisplayName("Randomness")]
  public class RandomnessController : WaterfallController
  {
    public delegate float NoiseFunction();

    public const string PerlinNoiseName = "perlin";
    public const string RandomNoiseName = "random";

    [Persistent] public Vector2 range;
    [Persistent] public string noiseType = RandomNoiseName;

    [Persistent] public bool randomSeed;
    [Persistent] public int seed;
    [Persistent] public float scale = 1f;
    [Persistent] public float minimum;
    [Persistent] public float speed = 1f;

    private NoiseFunction noiseFunc;

    public RandomnessController() : base() { }
    public RandomnessController(ConfigNode node) : base(node)
    {
    }


    public override void Initialize(ModuleWaterfallFX host)
    {
      base.Initialize(host);

      values = new float[1];

      if (noiseType == PerlinNoiseName)
      {
        noiseFunc = PerlinNoise;
        if (randomSeed)
        { 
          seed = Random.Range(0, int.MaxValue); 
        }
      }
      else if (noiseType == RandomNoiseName)
      {
        noiseFunc = RandomNoise;
      }
      else
        noiseFunc = RandomNoise;
    }

    public float RandomNoise() => Random.Range(range.x, range.y);

    public float PerlinNoise() => Mathf.PerlinNoise(seed + Time.time * speed, seed + Time.time * speed) * (scale - minimum) + minimum;

    protected override bool UpdateInternal()
    {
      // this isn't really correct; we should probably use the normal "changed" logic here - but the way things are currently set up, marking these as awake is really bad for performance
      // I think randomness is currently kind of broken anyway, it needs another pass since it's kind of a special case in the fixed-function pipeline
      // But if someone hooked a random controller directly up to a modifier then it wouldn't update properly.
      values[0] = noiseFunc();
      return false;
    }
  }
}

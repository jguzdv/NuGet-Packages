﻿namespace JGUZDV.AspNetCore.Hosting.FeatureManagement;

/// <summary>
/// Represents a list of features and their status
/// </summary>
public record FeatureList(IEnumerable<Feature> Features);



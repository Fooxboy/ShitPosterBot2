﻿using ShitPosterBot2.Shared.Models;

namespace ShitPosterBot2.Collector;

public interface IPostCollector
{
    public string Name { get; }
    public event Func<Post, Task> NewPostParsed;    
    
    public event Action<Exception, IPostCollector> PostCollectorCrashed;

    public Task RunCollectorAsync(ICollectorSettings settings);

    public void StopCollector();
}
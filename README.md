# atomizer
Create a thread-safe wrapper for any object.

```csharp
private static void Method() 
{
  var dictionary = Atomic.Instance(new Dictionary<int, int>());
  var tasks = Enumerable.Range(0, 1000)
    .Select(x => new Task(() => dictionary.Do(CriticalSection)))
    .ToList();

  tasks.ForEach(x => x.Start());
  tasks.ForEach(x => x.Wait());
  // does not throw.
}

private static void CriticalSection(Dictionary<int, int> x)
{
    if (!x.ContainsKey(0))
    {
        // pretend that it takes a long time to add this value, and without thread-safety some other thread might enter this block.
        Thread.Sleep(500);
        x.Add(0, 0);
    }
    else
    {
        x[0] += 5;
    }
}
```

namespace Api.Helpers;
using System.Collections.Concurrent;

public interface IOrderStore
{
    void Save(string orderId, object payload);
    object Get(string orderId);
    void Update(string orderId, object payload);
}

public class InMemoryOrderStore : IOrderStore
{
    private readonly ConcurrentDictionary<string, object> _store = new();
    public void Save(string orderId, object payload) => _store[orderId] = payload;
    public object Get(string orderId) => _store.TryGetValue(orderId, out var v) ? v : null;
    public void Update(string orderId, object payload) => _store[orderId] = payload;
}
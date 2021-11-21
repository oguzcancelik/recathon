using System;
using System.Collections.Generic;

namespace SpotifyGateway.Infrastructure.Contexts
{
    public abstract class SpotifyContext
    {
        private readonly HashSet<string> _set = new();

        protected void SetValue(string key, bool isValid, Action action)
        {
            if (isValid && !_set.Contains(key))
            {
                action();

                _set.Add(key);
            }
        }
    }
}
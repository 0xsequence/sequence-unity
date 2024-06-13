using UnityEngine;

namespace Sequence.Demo
{
    public interface ISearchable
    {
        public Sprite GetIcon();
        public string GetName();
        public Chain GetNetwork();
        public uint GetNumberOwned();
    }
}
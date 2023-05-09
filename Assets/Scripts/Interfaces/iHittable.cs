using UnityEngine;
namespace Interfaces.Hittable
{
    public interface IHittable
    {
        void WhenHit(GameObject other);
    }
}
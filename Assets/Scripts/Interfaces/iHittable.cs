using UnityEngine;
namespace Interfaces.Hittable
{
    public interface IHittable
    {
        void OnHit(GameObject other);
    }
}
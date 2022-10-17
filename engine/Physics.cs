using System.Numerics;
using WhiteWorld.engine.ecs.interfaces;
using WhiteWorld.engine.ecs.scripts;

namespace WhiteWorld.engine;

public partial class Engine {

    private static readonly Logger PhysicsLogger = GetLogger("Engine/Physics");

    public static List<Collider> ActiveColliders = new();
    public static List<Interactable> ActiveInteractables = new();

    public static T? Raycast<T>(IReadOnlyList<T> targets, Vector2 origin, Vector2 direction, float distance) where T : class, IViewport {
        DebugRay(origin, direction, distance);
        return targets.Select(target => {
            var t0X = (target.Position.X - origin.X) / direction.X;
            var t0Y = (target.Position.Y - origin.Y) / direction.Y;
            var t1X = (target.Position.X + target.Size.X - origin.X) / direction.X;
            var t1Y = (target.Position.Y + target.Size.Y - origin.Y) / direction.Y;

            DebugPoint(new Vector2(origin.X + t0X * direction.X, origin.Y + t0X * direction.Y));
            DebugPoint(new Vector2(origin.X + t0Y * direction.X, origin.Y + t0Y * direction.Y));

            var tMin = MathF.Max(MathF.Min(t0X, t1X), MathF.Min(t0Y, t1Y));
            var tMax = MathF.Min(MathF.Max(t0X, t1X), MathF.Max(t0Y, t1Y));

            if (tMax >= 0 && tMin <= tMax) { // intersecting
                var closest = tMin < 0 ?
                    new Vector2(origin.X + tMax * direction.X, origin.Y + tMax * direction.Y):
                    new Vector2(origin.X + tMin * direction.X, origin.Y + tMin * direction.Y);
                DebugPoint(closest);
                var dist = MathF.Sqrt(MathF.Pow(closest.X - origin.X, 2) + MathF.Pow(closest.Y - origin.Y, 2));
                if (dist <= distance) return new {
                    target,
                    distance = dist
                };
            }
            return null;
        }).MinBy(target => target?.distance)?.target;
    }
}
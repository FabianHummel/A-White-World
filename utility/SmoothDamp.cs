using System.Numerics;

namespace WhiteWorld.utility;

internal static class SmoothDamp {
    // Based on Unity Source Code which is based on Game Programming Gems 4 Chapter 1.10
	public static Vector2 Calc(Vector2 current, Vector2 target, ref Vector2 currentVelocity, float smoothTime, float maxSpeed, float deltaTime) {
		smoothTime = MathF.Max(0.0001F, smoothTime);
        var omega = 2F / smoothTime;

        var x = omega * deltaTime;
        var exp = 1F / (1F + x + 0.48F * x * x + 0.235F * x * x * x);

        var changeX = current.X - target.X;
        var changeY = current.Y - target.Y;
        var originalTo = target;

        // Clamp maximum speed
        var maxChange = maxSpeed * smoothTime;

        var maxChangeSq = maxChange * maxChange;
        var sqDist = changeX * changeX + changeY * changeY;
        if (sqDist > maxChangeSq) {
            var mag = MathF.Sqrt(sqDist);
            changeX = changeX / mag * maxChange;
            changeY = changeY / mag * maxChange;
        }

        target.X = current.X - changeX;
        target.Y = current.Y - changeY;

        var tempX = (currentVelocity.X + omega * changeX) * deltaTime;
        var tempY = (currentVelocity.Y + omega * changeY) * deltaTime;

        currentVelocity.X = (currentVelocity.X - omega * tempX) * exp;
        currentVelocity.Y = (currentVelocity.Y - omega * tempY) * exp;

        var outputX = target.X + (changeX + tempX) * exp;
        var outputY = target.Y + (changeY + tempY) * exp;

        // Prevent overshooting
        var origMinusCurrentX = originalTo.X - current.X;
        var origMinusCurrentY = originalTo.Y - current.Y;
        var outMinusOrigX = outputX - originalTo.X;
        var outMinusOrigY = outputY - originalTo.Y;

        if (origMinusCurrentX * outMinusOrigX + origMinusCurrentY * outMinusOrigY > 0) {
            outputX = originalTo.X;
            outputY = originalTo.Y;

            currentVelocity.X = (outputX - originalTo.X) / deltaTime;
            currentVelocity.Y = (outputY - originalTo.Y) / deltaTime;
        }
        
        return new Vector2(outputX, outputY);
    }
}
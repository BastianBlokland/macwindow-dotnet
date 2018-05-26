namespace NativeMacOS
{
    public static class FloatUtils
    {
        public static float Clamp(this float value, float min, float max)
		{
			if(value < min) value = min;
			if(value > max) value = max;
			return value;
		}
    }
}
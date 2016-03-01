public static class Extension
{
    public static bool IsUpOrDown(this Direction direction)
    {
        return direction == Direction.Up || direction == Direction.Down;
    }

    public static bool IsUpOrRight(this Direction direction)
    {
        return direction == Direction.Up || direction == Direction.Right;
    }
}

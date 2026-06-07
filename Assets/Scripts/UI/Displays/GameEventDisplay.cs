public class GameEventDisplay : Display<GameEvent>
{
    protected override string FormatValue(GameEvent value)
    {
        string labelContent = value == null ? "None" : value.Name;
        return labelContent;
    }
}
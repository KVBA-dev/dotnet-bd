namespace Game.Entities;

public interface IEntityController {
    public float Horizontal { get; }
    public float Vertical { get; }
    public void GetAxes();
    public bool Enabled { get; set; }
}

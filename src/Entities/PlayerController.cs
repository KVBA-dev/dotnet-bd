namespace Game.Entities;
public sealed class PlayerController : IEntityController {
    public float Horizontal => horiz;
    public float Vertical => vert;

    private float horiz;
    private float vert;


    public void GetAxes() {
        horiz = 0;
        if (Input.Left) {
            horiz -= 1;
        }
        if (Input.Right) {
            horiz += 1;
        }
        vert = 0;
        if (Input.Jump) {
            vert = 1;
        }
    }
}
public enum ConstraintType {
    DISTANCE,
}

public struct Constraint {
    int[] vertices;
    float restValue;
    float currentValue;
    ConstraintType type;

    public Constraint(int[] vertices, float restValue, ConstraintType type) {
        this.vertices = vertices;
        this.restValue = restValue;
        currentValue = restValue;
        this.type = type;
    }
}
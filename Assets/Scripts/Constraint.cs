public enum EConstraintType {
    DEFAULT = -1,
    DISTANCE = 0,
    BENDING = 1,
    VOLUME = 2
}

public struct Constraint {
    int[] vertices;
    float restValue;
    float currentValue;
    EConstraintType constraintType;

    public Constraint(int[] vertices, float restValue, EConstraintType type) {
        this.vertices = vertices;
        this.restValue = restValue;
        this.currentValue = restValue;
        this.constraintType = type;
    }

    public int[] getVertices() {
        return vertices;
    }

    public float getRestValue() {
        return restValue;
    }

    public float getCurrentValue() {
        return currentValue;
    }

    public EConstraintType getConstraintType() {
        return constraintType;
    }
}
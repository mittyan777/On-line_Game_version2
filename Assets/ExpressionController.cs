using UnityEngine;
using UniVRM10;

public class ExpressionController : MonoBehaviour
{
    [SerializeField] private Vrm10Instance _vrm;
    private Vrm10RuntimeExpression _expression;

    public ExpressionPreset preset;
    [Range(0, 1f)] public float _value;

    void Awake()
    {
        _expression = _vrm.Runtime.Expression;
    }

    void Update()
    {
        _expression.SetWeight(ExpressionKey.CreateFromPreset(preset), _value);
    }
}
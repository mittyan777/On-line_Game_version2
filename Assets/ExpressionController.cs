using UnityEngine;
using UniVRM10;

public class ExpressionController : MonoBehaviour
{
    [SerializeField] private Vrm10Instance _vrm;
    private Vrm10RuntimeExpression _expression;

    [Header("Settings")]
    public ExpressionPreset preset;

    // Customを選択した場合はここに表情名を入力する（例: "Smile", "Blink_L"など）
    public string customName;

    [Range(0, 1f)] public float _value;

    void Awake()
    {
        // 念のためnullチェック
        if (_vrm != null && _vrm.Runtime != null)
        {
            _expression = _vrm.Runtime.Expression;
        }
    }

    void Update()
    {
        if (_expression == null) return;

        ExpressionKey key;

        // Customかどうかでキーの作り方を変える
        if (preset == ExpressionPreset.custom)
        {
            // 名前が空だとエラーになるのでチェック
            if (string.IsNullOrEmpty(customName))
            {
                return;
            }
            // カスタムの場合は名前指定でキーを作る
            key = ExpressionKey.CreateCustom(customName);
        }
        else
        {
            // 標準プリセット（Joy, Angry, Neutralなど）の場合はこちら
            key = ExpressionKey.CreateFromPreset(preset);
        }

        // ウェイトを適用
        _expression.SetWeight(key, _value);
    }
}
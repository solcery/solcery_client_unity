using Newtonsoft.Json.Linq;
using Solcery.BrickInterpretation;
using Solcery.BrickInterpretation.Actions;
using Solcery.BrickInterpretation.Conditions;
using Solcery.BrickInterpretation.Values;
using TMPro;
using UnityEngine;

namespace Solcery
{
    public class SetText : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text text;
        
        // Start is called before the first frame update
        void Start()
        {
            text.text = "Prod";
#if DEV
            text.text = "Dev";
#endif

            var brickService = BrickService.Create();
            // Value bricks
            brickService.RegistrationBrickType("brick_value_const", BrickValueConst.Create);
            // Action bricks
            brickService.RegistrationBrickType("brick_action_void", BrickActionVoid.Create);
            brickService.RegistrationBrickType("brick_action_set", BrickActionSet.Create);
            brickService.RegistrationBrickType("brick_action_conditional", BrickActionConditional.Create);
            // Condition bricks
            brickService.RegistrationBrickType("brick_condition_const", BrickConditionConst.Create);
            brickService.RegistrationBrickType("brick_condition_not", BrickConditionNot.Create);
            brickService.RegistrationBrickType("brick_condition_and", BrickConditionAnd.Create);
            brickService.RegistrationBrickType("brick_condition_or", BrickConditionOr.Create);
            brickService.RegistrationBrickType("brick_condition_equal", BrickConditionEqual.Create);
            brickService.RegistrationBrickType("brick_condition_greater_than", BrickConditionGreaterThan.Create);
            brickService.RegistrationBrickType("brick_condition_lesser_than", BrickConditionLesserThan.Create);
            
            var json = JObject.Parse(Resources.Load<TextAsset>("test").text);
            var result = brickService.ExecuteConditionBrick(json, Context.Create());
            Debug.Log($"Brick run result {result}");
            brickService.Cleanup();
        }
    }
}

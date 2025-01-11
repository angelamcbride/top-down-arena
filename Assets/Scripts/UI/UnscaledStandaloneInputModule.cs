using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// A custom StandaloneInputModule that processes UI pointer and navigation
/// events at unscaled time, so Time.timeScale = 0 won't freeze your menu.
/// 
/// Usage Steps:
/// 1. Remove/disable the default StandaloneInputModule on your EventSystem.
/// 2. Attach this script to the same GameObject as the EventSystem.
/// 3. That's it! Now UI interaction keeps working even when Time.timeScale = 0.
/// </summary>
[AddComponentMenu("Event/Unscaled Standalone Input Module")]
public class UnscaledStandaloneInputModule : StandaloneInputModule
{
    // These are needed because they're private in the base StandaloneInputModule.
    // We copy them here so we can customize time usage for navigation events.
    [SerializeField] private float m_InputActionsPerSecond = 10f;
    [SerializeField] private float m_RepeatDelay = 0.5f;

    private float m_PrevActionTime;
    private int m_ConsecutiveMoveCount;
    private Vector2 m_LastMoveVector;

    // Optional custom axis names (same defaults as Unity's EventSystem).
    // You can change them in the Inspector if needed.
    [SerializeField] private string m_HorizontalAxis = "Horizontal";
    [SerializeField] private string m_VerticalAxis = "Vertical";

    /// <summary>
    /// We override StandaloneInputModule.Process() using 'public override'
    /// to match the base class signature. Just calling base.Process() is
    /// often enough to keep pointer hover/click working at timeScale=0.
    /// </summary>
    public override void Process()
    {
        base.Process();
    }

    /// <summary>
    /// By default, StandaloneInputModule uses Time.time for navigation repeating.
    /// Here we override it to use unscaled time, so keyboard/controller navigation
    /// also works when paused (timeScale=0).
    /// </summary>
    protected override bool SendMoveEventToSelectedObject()
    {
        // Use unscaled time for our repeated navigation logic:
        float time = Time.unscaledTime;

        Vector2 movement = GetRawMoveVector();

        if (Mathf.Approximately(movement.x, 0f) && Mathf.Approximately(movement.y, 0f))
        {
            m_ConsecutiveMoveCount = 0;
            return false;
        }

        bool similarDir = Vector2.Dot(movement, m_LastMoveVector) > 0;

        // If still moving in the same direction, check if it's time to repeat:
        if (similarDir && m_ConsecutiveMoveCount == 1)
        {
            if (time <= m_PrevActionTime + m_RepeatDelay)
                return false;
        }
        else
        {
            // If direction changed or we've already done the initial delay,
            // apply the "inputs per second" rate.
            if (time <= m_PrevActionTime + 1f / m_InputActionsPerSecond)
                return false;
        }

        var axisEventData = GetAxisEventData(movement.x, movement.y, 0.6f);
        if (axisEventData.moveDir != MoveDirection.None)
        {
            ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, axisEventData, ExecuteEvents.moveHandler);

            if (!similarDir)
                m_ConsecutiveMoveCount = 0;

            m_ConsecutiveMoveCount++;
            m_PrevActionTime = time;
            m_LastMoveVector = movement;
        }
        else
        {
            m_ConsecutiveMoveCount = 0;
        }

        return axisEventData.used;
    }

    /// <summary>
    /// A copy of the base class's private GetRawMoveVector() logic,
    /// so we can reference it in our override above.
    /// You can customize axis names in the Inspector.
    /// </summary>
    private Vector2 GetRawMoveVector()
    {
        Vector2 move = Vector2.zero;
        move.x = Input.GetAxisRaw(m_HorizontalAxis);
        move.y = Input.GetAxisRaw(m_VerticalAxis);

        if (Input.GetButtonDown(m_HorizontalAxis))
        {
            if (move.x < 0f) move.x = -1f;
            if (move.x > 0f) move.x = 1f;
        }

        if (Input.GetButtonDown(m_VerticalAxis))
        {
            if (move.y < 0f) move.y = -1f;
            if (move.y > 0f) move.y = 1f;
        }

        return move;
    }
}

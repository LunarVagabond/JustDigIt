namespace CustomInputActions
{

  /* Import this as: 
      using IA = CustomInputActions.InputActions;
  */
  public class InputActions
  {


    #region  Movement
    public const string MOVE_LEFT = "move_left";
    public const string MOVE_RIGHT = "move_right";
    public const string MOVE_UP = "move_up";
    public const string MOVE_DOWN = "move_down";
    public const string JUMP = "jump";
    public const string INTERACT = "interact";
    #endregion

    #region Actions
    public const string MINE = "mine";
    public const string TOGGLE_MOUSE_VISIBILITY = "toggle_mouse_visibility";
    public const string TURN_LAMP_UP = "increase_light";
    public const string TURN_LAMP_DOWN = "decrease_light";
    #endregion

    #region UI
    public const string MENU_TOGGLE = "menu";
    #endregion
  }
}
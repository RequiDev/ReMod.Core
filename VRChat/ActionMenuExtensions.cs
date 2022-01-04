namespace ReMod.Core.VRChat
{
    public static class ActionMenuExtensions
    {
        public static bool IsOpen(this ActionMenuDriver actionMenuDriver)
        {
            return actionMenuDriver.GetLeftOpener().IsOpen() || actionMenuDriver.GetRightOpener().IsOpen();
        }

        public static bool IsOpen(this ActionMenuOpener amo)
        {
            return amo.field_Private_Boolean_0;
        }

        public static ActionMenuOpener GetLeftOpener(this ActionMenuDriver actionMenuDriver)
        {
            if (actionMenuDriver.field_Public_ActionMenuOpener_0.field_Public_Hand_0 ==
                ActionMenuOpener.Hand.Left)
                return actionMenuDriver.field_Public_ActionMenuOpener_0;
            return actionMenuDriver.field_Public_ActionMenuOpener_1;
        }

        public static ActionMenuOpener GetRightOpener(this ActionMenuDriver actionMenuDriver)
        {
            if (actionMenuDriver.field_Public_ActionMenuOpener_1.field_Public_Hand_0 ==
                ActionMenuOpener.Hand.Right)
                return actionMenuDriver.field_Public_ActionMenuOpener_1;
            return actionMenuDriver.field_Public_ActionMenuOpener_0;
        }
    }
}

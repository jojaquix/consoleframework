using System;
using ConsoleFramework.Controls;
using ConsoleFramework.Core;

namespace ConsoleFramework.Events {

    public delegate void MouseEventHandler(object sender, MouseEventArgs e);

    public delegate void MouseButtonEventHandler(object sender, MouseButtonEventArgs e);

    public delegate void MouseWheelEventHandler(object sender, MouseWheelEventArgs e);

    public enum MouseButtonState
    {
        Released,
        Pressed
    }

    public class MouseWheelEventArgs : MouseEventArgs
    {
        public MouseWheelEventArgs(object source, RoutedEvent routedEvent, Point rawPosition,
                                    MouseButtonState leftButton, MouseButtonState middleButton,
                                    MouseButtonState rightButton, int delta)
            : base(source, routedEvent, rawPosition, leftButton, middleButton, rightButton) {
            Delta = delta;
        }

        public int Delta {
            get;
            private set;
        }
    }

    public enum MouseButton
    {
        Left,
        Middle,
        Right
    }

    public class MouseButtonEventArgs : MouseEventArgs
    {
        private readonly MouseButton button;
        private readonly int clickCount;

        public MouseButtonEventArgs(object source, RoutedEvent routedEvent)
            : base(source, routedEvent) {
        }

        public MouseButtonEventArgs(object source, RoutedEvent routedEvent, Point rawPosition,
                                    MouseButtonState leftButton, MouseButtonState middleButton,
                                    MouseButtonState rightButton,
                                    MouseButton button, int clickCount = 1)
            : base(source, routedEvent, rawPosition, leftButton, middleButton, rightButton) {
            this.button = button;
            this.clickCount = clickCount;
        }

        public MouseButtonState ButtonState {
            get {
                switch (button) {
                    case MouseButton.Left:
                        return LeftButton;
                    case MouseButton.Middle:
                        return MiddleButton;
                    case MouseButton.Right:
                        return RightButton;
                }
                throw new InvalidOperationException("This code should not be reached.");
            }
        }

        public MouseButton ChangedButton {
            get { return button; }
        }

        public int ClickCount {
            get { return clickCount; }
        }
    }


    public class MouseEventArgs : RoutedEventArgs
    {
        public MouseEventArgs(object source, RoutedEvent routedEvent) : base(source, routedEvent) {
        }

        public MouseEventArgs(object source, RoutedEvent routedEvent, Point rawPosition,
                              MouseButtonState leftButton, MouseButtonState middleButton, MouseButtonState rightButton)
            : base(source, routedEvent) {
            //
            this.rawPosition = rawPosition;
            this.LeftButton = leftButton;
            this.MiddleButton = middleButton;
            this.RightButton = rightButton;
        }

        private readonly Point rawPosition;
        public Point RawPosition {
            get {
                return rawPosition;
            }
        }

        /// <summary>
        /// ���������� ��������������� ���������� ������������ ���������� ��������.
        /// ���� ������� ����������� ����, �� ���������� ����� ������� ���������� �����
        /// �������� �� ������� �������� (������������� ��� ������ ActualWidth/ActualHeight).
        /// </summary>
        public Point GetPosition(Control relativeTo) {
            return Control.TranslatePoint(null, rawPosition, relativeTo);
        }

        public MouseButtonState LeftButton {
            get;
            private set;
        }

        public MouseButtonState MiddleButton {
            get;
            private set;
        }

        public MouseButtonState RightButton {
            get;
            private set;
        }
    }
}
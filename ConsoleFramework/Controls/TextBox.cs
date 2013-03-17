﻿using System;
using System.Diagnostics;
using ConsoleFramework.Core;
using ConsoleFramework.Events;
using ConsoleFramework.Native;

namespace ConsoleFramework.Controls {
    public class TextBox : Control {
        public TextBox() {
            KeyDown += TextBox_KeyDown;
            MouseDown += OnMouseDown;
            GotKeyboardFocus += OnGotKeyboardFocus;
            LostKeyboardFocus += OnLostKeyboardFocus;
            CursorVisible = true;
            CursorPosition = new Point(1, 0);
            Focusable = true;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs args) {
            Point point = args.GetPosition(this);
            if (point.X > 0 && point.X - 1 < Size) {
                int x = point.X - 1;
                if (!String.IsNullOrEmpty(text)) {
                    if (x <= text.Length)
                        cursorPosition = x;
                    else {
                        cursorPosition = text.Length;
                    }
                    CursorPosition = new Point(cursorPosition + 1, 0);
                }
                args.Handled = true;
            }
        }

        private void OnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs args) {
        }

        private void OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs args) {
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs args) {
            // todo : add right alt & ctrl support
            ConsoleKeyInfo keyInfo = new ConsoleKeyInfo(args.UnicodeChar, (ConsoleKey) args.wVirtualKeyCode,
                                                        (args.dwControlKeyState & ControlKeyState.SHIFT_PRESSED) ==
                                                        ControlKeyState.SHIFT_PRESSED,
                                                        (args.dwControlKeyState & ControlKeyState.LEFT_ALT_PRESSED) ==
                                                        ControlKeyState.LEFT_ALT_PRESSED,
                                                        (args.dwControlKeyState & ControlKeyState.LEFT_CTRL_PRESSED) ==
                                                        ControlKeyState.LEFT_CTRL_PRESSED);
            if (!char.IsControl(keyInfo.KeyChar)) {
                // insert keychar into a text string according to cursorPosition and offset
                if (text != null) {
                    string leftPart = text.Substring(0, cursorPosition + displayOffset);
                    string rightPart = text.Substring(cursorPosition + displayOffset);
                    Text = leftPart + keyInfo.KeyChar + rightPart;
                } else {
                    Text = keyInfo.KeyChar.ToString();
                }
                if (cursorPosition + 1 < Size) {
                    cursorPosition++;
                    CursorPosition = new Point(cursorPosition + 1, 0);
                } else {
                    displayOffset++;
                }
            } else {
                if (keyInfo.Key == ConsoleKey.Delete) {
                    if (!String.IsNullOrEmpty(text) && displayOffset + cursorPosition < text.Length) {
                        string leftPart = text.Substring(0, cursorPosition + displayOffset);
                        string rightPart = text.Substring(cursorPosition + displayOffset + 1);
                        Text = leftPart + rightPart;
                        //
                    } else {
                        Console.Beep();
                    }
                }
                if (keyInfo.Key == ConsoleKey.Backspace) {
                    if (!String.IsNullOrEmpty(text) && (displayOffset != 0 || cursorPosition != 0)) {
                        string leftPart = text.Substring(0, cursorPosition + displayOffset - 1);
                        string rightPart = text.Substring(cursorPosition + displayOffset);
                        Text = leftPart + rightPart;
                        if (displayOffset > 0)
                            displayOffset--;
                        else {
                            if (cursorPosition > 0) {
                                cursorPosition--;
                                CursorPosition = new Point(cursorPosition + 1, 0);
                            }
                        }
                    } else {
                        Console.Beep();
                    }
                }
                if (keyInfo.Key == ConsoleKey.LeftArrow) {
                    if (!String.IsNullOrEmpty(text) && (displayOffset != 0 || cursorPosition != 0)) {
                        if (cursorPosition > 0) {
                            cursorPosition--;
                            CursorPosition = new Point(cursorPosition + 1, 0);
                        } else {
                            if (displayOffset > 0) {
                                displayOffset--;
                                Invalidate();
                            }
                        }
                    } else {
                        Console.Beep();
                    }
                }
                if (keyInfo.Key == ConsoleKey.RightArrow) {
                    if (!String.IsNullOrEmpty(text) && displayOffset + cursorPosition < text.Length) {
                        if (cursorPosition + 1 < Size) {
                            cursorPosition++;
                            CursorPosition = new Point(cursorPosition + 1, 0);
                        } else {
                            if (displayOffset + cursorPosition < text.Length) {
                                displayOffset++;
                                Invalidate();
                            }
                        }
                    } else {
                        Console.Beep();
                    }
                }
                if (keyInfo.Key == ConsoleKey.Home) {
                    if (displayOffset != 0 || cursorPosition != 0) {
                        displayOffset = 0;
                        cursorPosition = 0;
                        CursorPosition = new Point(cursorPosition + 1, 0);
                        Invalidate();
                    } else {
                        Console.Beep();
                    }
                }
                if (keyInfo.Key == ConsoleKey.End) {
                    if (!String.IsNullOrEmpty(text) && cursorPosition + displayOffset < text.Length) {
                        displayOffset = text.Length >= Size ? text.Length - Size + 1 : 0;
                        cursorPosition = text.Length >= Size ? Size - 1 : text.Length;
                        CursorPosition = new Point(cursorPosition + 1, 0);
                        Invalidate();
                    } else {
                        Console.Beep();
                    }
                }
            }
            //Debugger.Log(0, "", String.Format("cursorPos : {0} offset {1}\n", cursorPosition, displayOffset));
        }

        private string text;

        public string Text {
            get {
                return text;
            }
            set {
                if (text != value) {
                    text = value;
                    Invalidate();
                }
            }
        }

        public int MaxLenght {
            get;
            set;
        }

        public int Size {
            get;
            set;
        }

        protected override Size MeasureOverride(Size availableSize) {
            return new Size(Size + 2, 1);
        }

        protected override Size ArrangeOverride(Size finalSize) {
            return new Size(Size + 2, 1);
        }

        // this fields describe the whole state of textbox
        private int displayOffset;
        private int cursorPosition;
        // -1 if no selection started
        private int startSelection;

        public override void Render(RenderingBuffer buffer) {
            ushort attr = Color.Attr(Color.White, Color.DarkBlue);
            buffer.FillRectangle(0, 0, Size + 2, ActualHeight, ' ', attr);
            if (null != text) {
                for (int i = displayOffset; i < text.Length; i++) {
                    if (i - displayOffset < Size) {
                        buffer.SetPixel(1 + i - displayOffset, 0, text[i]);
                    }
                }
            }
            ushort arrowsAttr = Color.Attr(Color.Green, Color.DarkBlue);
            if (displayOffset > 0)
                buffer.SetPixel(0, 0, '<', (CHAR_ATTRIBUTES) arrowsAttr);
            if (!String.IsNullOrEmpty(text) && Size + displayOffset < text.Length)
                buffer.SetPixel(Size + 1, 0, '>', (CHAR_ATTRIBUTES) arrowsAttr);
        }
    }
}
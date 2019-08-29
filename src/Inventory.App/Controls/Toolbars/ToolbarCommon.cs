#region copyright
// ******************************************************************
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************
#endregion

using System;

namespace Inventory.Controls
{
    public enum ToolbarButton
    {
        Back,
        New,
        Edit,
        Delete,
        Cancel,
        Save,
        Select,
        Refresh
    }

    public enum ListToolbarMode
    {
        Default,
        Cancel,
        CancelDelete
    }

    public enum DetailToolbarMode
    {
        Default,
        BackEditDelete,
        CancelSave
    }

    public class ToolbarButtonClickEventArgs : EventArgs
    {
        public ToolbarButtonClickEventArgs(ToolbarButton button)
        {
            ClickedButton = button;
        }

        public ToolbarButton ClickedButton { get; }
    }

    public delegate void ToolbarButtonClickEventHandler(object sender, ToolbarButtonClickEventArgs e);
}

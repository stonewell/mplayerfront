using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace MPlayerFront
{
    public class KeysComboBox : TextBox
    {
        public KeysComboBox()
        {
        }

        //protected override void OnKeyPress(KeyPressEventArgs e)
        //{
        //    Text = e.KeyChar.ToString();
        //    e.Handled = true;
        //}

        //protected override void OnKeyDown(KeyEventArgs e)
        //{
        //    e.SuppressKeyPress = true;
        //}

        //protected override void OnKeyUp(KeyEventArgs e)
        //{
        //    base.OnKeyUp(e);
        //}

        //protected override bool IsInputKey(Keys keyData)
        //{
        //    bool result = keyData == Keys.Enter ||
        //        keyData == Keys.Tab ||
        //        keyData == Keys.Return ||
        //        keyData == Keys.Escape ||
        //        keyData == Keys.Up ||
        //        keyData == Keys.Down ||
        //        keyData == Keys.Left ||
        //        keyData == Keys.Right ||
        //        keyData == Keys.PageDown ||
        //        keyData == Keys.PageUp ||
        //        base.IsInputKey(keyData);

        //    return result;
        //}

        //protected override bool IsInputChar(char charCode)
        //{
        //    return base.IsInputChar(charCode);
        //}
    }
}

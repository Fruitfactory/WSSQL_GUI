﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WSPreview.PreviewHandler.PreviewHandlerFramework;
using ScintillaNET;

namespace WSPreview.PreviewHandler.Controls
{
    [KeyControl(ControlsKey.Source)]
    public partial class SourceViewer : UserControl,IPreviewControl
    {
#region fields

        private Scintilla _scintilla;

#endregion

        public SourceViewer()
        {
            InitializeComponent();
            _scintilla = new Scintilla();
            _scintilla.Dock = DockStyle.Fill;
            Controls.Add(_scintilla);
        }

        #region public

        public void LoadFile(string filename)
        {
            if (_scintilla == null)
                return;
            _scintilla.UndoRedo.EmptyUndoBuffer();
            _scintilla.Modified = false;
            ConfigureScintilla();
            SetLanguage(filename);
            _scintilla.Text = File.ReadAllText(filename);
        }

        public void LoadFile(Stream stream)
        {
        }

        public void Clear()
        {
            if (_scintilla != null)
            {
                _scintilla.Text = string.Empty;
            }
        }

        #endregion

        #region private

        private void ConfigureScintilla()
        {
            if(_scintilla == null)
                return;
            _scintilla.Margins.Margin0.Width = 35;
        }

        private void SetLanguage(string filename)
        {
            if (_scintilla == null)
                return;
            string ext = Path.GetExtension(filename).ToLower();
            ext = ext.Substring(1);
            _scintilla.ConfigurationManager.Language = "";
            switch(ext)
            {
                case "cpp":
                case "c":
                case "h":
                case "hpp":
                    _scintilla.ConfigurationManager.Language = "cpp";
                    break;
                case "java":
                case "js":
                    _scintilla.ConfigurationManager.Language = "js";
                    break;
                case "bat":
                    _scintilla.ConfigurationManager.Language = "Batch";
                    break;
                case "asm":
                    _scintilla.ConfigurationManager.Language = "Assembly";
                    break;
                case "css":
                    _scintilla.ConfigurationManager.Language = "CSS";
                    break;
                case "cs":
                    _scintilla.ConfigurationManager.Language = "cs";
                    break;
                case "vb":
                    _scintilla.ConfigurationManager.Language = "vb";
                    break;
            }

            
        }


        #endregion


    }
}
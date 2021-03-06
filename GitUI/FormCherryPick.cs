﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using GitCommands;
using ResourceManager.Translation;

namespace GitUI
{
    public partial class FormCherryPick : GitModuleForm
    {
        private readonly TranslationString _noRevisionSelectedMsgBox =
            new TranslationString("Select 1 revision to pick.");
        private readonly TranslationString _noRevisionSelectedMsgBoxCaption =
            new TranslationString("Cherry pick");
        private readonly TranslationString _cmdExecutedMsgBox =
            new TranslationString("Command executed");
        private readonly TranslationString _cmdExecutedMsgBoxCaption =
            new TranslationString("Cherry pick");

        private FormCherryPick()
            : this(null)
        {
        }

        public FormCherryPick(GitUICommands aCommands)
            : base(aCommands)
        {
            InitializeComponent();
            Translate();
        }

        private void FormCherryPick_Load(object sender, EventArgs e)
        {
            RevisionGrid.Load();
        }

        private void CherryPick_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            if (RevisionGrid.GetSelectedRevisions().Count != 1)
            {
                MessageBox.Show(this, _noRevisionSelectedMsgBox.Text, _noRevisionSelectedMsgBoxCaption.Text);
                return;
            }
            bool formClosed = false;
            List<string> arguments = new List<string>();
            bool IsMerge = Module.IsMerge(RevisionGrid.GetSelectedRevisions()[0].Guid);
            if (IsMerge && !autoParent.Checked)
            {
                GitRevision[] ParentsRevisions = Module.GetParents(RevisionGrid.GetSelectedRevisions()[0].Guid);
                using (var choose = new FormCherryPickMerge(ParentsRevisions))
                {
                    choose.ShowDialog(this);
                    if (choose.OkClicked)
                        arguments.Add("-m " + (choose.ParentsList.SelectedItems[0].Index + 1));
                    else
                        formClosed = true;
                }
            }
            else if (IsMerge)
                arguments.Add("-m 1");

            if (checkAddReference.Checked)
                arguments.Add("-x");

            if (!formClosed)
            {
                MessageBox.Show(this, _cmdExecutedMsgBox.Text + " " + Environment.NewLine + Module.CherryPick(RevisionGrid.GetSelectedRevisions()[0].Guid, AutoCommit.Checked, string.Join(" ", arguments.ToArray())), _cmdExecutedMsgBoxCaption.Text);
                MergeConflictHandler.HandleMergeConflicts(UICommands, this, AutoCommit.Checked);
                RevisionGrid.RefreshRevisions();
                Cursor.Current = Cursors.Default;
            }
        }
    }
}

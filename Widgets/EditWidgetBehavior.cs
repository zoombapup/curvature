﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Curvature
{
    public partial class EditWidgetBehavior : UserControl, IInputBroker
    {
        private Behavior EditBehavior;


        public EditWidgetBehavior()
        {
            InitializeComponent();

            foreach (var e in Enum.GetValues(typeof(Behavior.ActionType)))
            {
                ActionComboBox.Items.Add(e);
            }

            ActionComboBox.SelectedIndex = 0;
        }

        internal void Attach(Behavior behavior)
        {
            foreach (Control ctl in ScoreLayoutPanel.Controls)
                ctl.Dispose();

            ScoreLayoutPanel.Controls.Clear();


            foreach (Control ctl in InputFlowPanel.Controls)
                ctl.Dispose();

            InputFlowPanel.Controls.Clear();


            EditBehavior = behavior;
            NameEditWidget.Attach("Behavior", EditBehavior);

            BehaviorWeightEditBox.Value = (decimal)EditBehavior.Weight;

            ActionComboBox.SelectedIndex = (int)EditBehavior.Action;
            CanTargetSelfCheckBox.Checked = EditBehavior.CanTargetSelf;
            CanTargetOthersCheckBox.Checked = EditBehavior.CanTargetOthers;

            var inputs = new HashSet<InputAxis>();
            foreach (var consideration in EditBehavior.Considerations)
            {
                if (consideration.Input != null)
                {
                    inputs.Add(consideration.Input);
                    ScoreLayoutPanel.Controls.Add(new EditWidgetConsiderationScore(consideration, this));
                }
            }

            foreach (var input in inputs)
            {
                var widget = new EditWidgetConsiderationInput(input, this);
                widget.Tag = input;
                InputFlowPanel.Controls.Add(widget);
            }

            RefreshInputs();
        }

        public double GetInputValue(InputAxis axis)
        {
            foreach (EditWidgetConsiderationInput input in InputFlowPanel.Controls)
            {
                if (input.Tag == axis)
                    return input.GetNormalizedValue();
            }

            return 0.0;
        }

        public double GetInputValue(InputAxis axis, Scenario.Context context)
        {
            return 0.0;
        }

        public void RefreshInputs()
        {
            ScoreListView.Items.Clear();

            double finalScore = (double)BehaviorWeightEditBox.Value;
            double compensationFactor = 1.0 - (1.0 / (double)ScoreLayoutPanel.Controls.Count);

            var weightItem = new ListViewItem(new string[] { $"{finalScore:f3}", "Behavior weight" });
            weightItem.Group = ScoreListView.Groups[1];

            ScoreListView.Items.Add(weightItem);
     
            foreach (EditWidgetConsiderationScore score in ScoreLayoutPanel.Controls)
            {
                score.Refresh();

                string considerationName = score.GetName();
                double considerationScore = score.GetValue();
                double modification = (1.0 - considerationScore) * compensationFactor;

                if (CompensationCheckBox.Checked)
                    considerationScore = considerationScore + (modification * considerationScore);

                var item = new ListViewItem(new string[] { $"{considerationScore:f3}", $"{considerationName}" });
                item.Group = ScoreListView.Groups[0];

                ScoreListView.Items.Add(item);
                finalScore *= considerationScore;
            }

            if (MomentumBonusCheckBox.Checked)
            {
                finalScore *= 1.25;

                var item = new ListViewItem(new string[] { "1.250", "Momentum bonus" });
                item.Group = ScoreListView.Groups[1];

                ScoreListView.Items.Add(item);
            }

            FinalScoreLabel.Text = $"Final Score = {finalScore:f3}";
        }

        private void MomentumBonusCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            RefreshInputs();
        }

        private void BehaviorWeightEditBox_ValueChanged(object sender, EventArgs e)
        {
            if (EditBehavior != null)
                EditBehavior.Weight = (double)BehaviorWeightEditBox.Value;

            RefreshInputs();
        }

        private void CompensationCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            RefreshInputs();
        }

        private void ActionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (EditBehavior != null)
                EditBehavior.Action = (Behavior.ActionType)ActionComboBox.SelectedIndex;
        }

        private void CanTargetSelfCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (EditBehavior != null)
                EditBehavior.CanTargetSelf = CanTargetSelfCheckBox.Checked;
        }

        private void CanTargetOthersCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (EditBehavior != null)
                EditBehavior.CanTargetOthers = CanTargetOthersCheckBox.Checked;
        }
    }
}

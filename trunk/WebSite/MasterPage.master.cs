﻿using System;
using Ra.Widgets;
using Entities;
using Ra.Extensions;

public partial class MasterPage : System.Web.UI.MasterPage
{
    protected override void OnInit(EventArgs e)
    {
        if (Operator.TryLoginFromCookie())
        {
            // Successfully logged in from cookie...
            Login();
        }
        base.OnInit(e);
    }

    protected void goToProfile_Click(object sender, EventArgs e)
    {
        // TODO: Implement...
        profileWindow.Visible = true;
        changeFriendlyName.Text = Operator.Current.FriendlyName;
        changeFriendlyName.Select();
        changeFriendlyName.Focus();
    }

    protected void saveProfile_Click(object sender, EventArgs e)
    {
        Operator.Current.FriendlyName = changeFriendlyName.Text;
        Operator.Current.Save();
        profileWindow.Visible = false;
    }

    protected void changeFriendlyName_EscPressed(object sender, EventArgs e)
    {
        profileWindow.Visible = false;
    }

    protected void auto_RetrieveAutoCompleterItems(object sender, AutoCompleter.RetrieveAutoCompleterItemsEventArgs e)
    {
        if (e.Query.Trim() == string.Empty)
            return;
        foreach (QuizItem idx in QuizItem.Search(e.Query))
        {
            AutoCompleterItem a = new AutoCompleterItem();
            System.Web.UI.WebControls.Literal lit = new System.Web.UI.WebControls.Literal();
            string tmpHeader = idx.Header;
            foreach (string idxStr in e.Query.Split(' '))
            {
                int index = tmpHeader.IndexOf(idxStr, StringComparison.InvariantCultureIgnoreCase);
                if (index != -1)
                {
                    tmpHeader = tmpHeader.Insert(index + idxStr.Length, "</span>");
                    tmpHeader = tmpHeader.Insert(index, "<span class=\"found\">");
                }
            }
            lit.Text = string.Format("<a href=\"{0}\">{1}</a>", idx.Url, tmpHeader);
            a.Controls.Add(lit);
            e.Controls.Add(a);
        }
    }

    protected void register_UserRegistered(object sender, EventArgs e)
    {
        infoLabel.Text = "Successfully registered";
        new EffectFadeIn(infoLabel, 1000)
            .ChainThese(new EffectHighlight(infoLabel, 500))
            .Render();
        infoTimer.Enabled = true;
        Login();
    }

    protected void infoTimer_Tick(object sender, EventArgs e)
    {
        new EffectHighlight(infoLabel, 500)
            .ChainThese(new EffectFadeOut(infoLabel, 1000))
            .Render();
        infoTimer.Enabled = false;
    }

    protected void askQuestion_Click(object sender, EventArgs e)
    {
        ask.ShowAskQuestion();
    }

    protected void ask_QuestionAsked(object sender, EventArgs e)
    {
        IDefault def = (Page as IDefault);
        if (def != null)
            def.QuestionsUpdated();
    }

    protected void loginBtn_Click(object sender, EventArgs e)
    {
        login.ShowLogin();
    }

    protected void registerBtn_Click(object sender, EventArgs e)
    {
        register.ShowRegisterWindow();
    }

    protected void login_LoggedIn(object sender, EventArgs e)
    {
        Login();
        new EffectHighlight(loginPnl, 500).Render();
    }

    private void Login()
    {
        loginBtn.Visible = false;
        registerBtn.Visible = false;
        logouBtn.Visible = true;
        askQuestion.Visible = true;
        goToProfile.Visible = true;
        goToProfile.Text = "Profile";
        goToProfile.Text += "<span class=\"profileName\">" + Operator.Current.FriendlyName + "</span>";
        logouBtn.Text = "Logout";
    }

    protected void logouBtn_Click(object sender, EventArgs e)
    {
        Operator.Logout();
        loginBtn.Visible = true;
        registerBtn.Visible = true;
        goToProfile.Visible = false;
        logouBtn.Visible = false;
        askQuestion.Visible = false;
        new EffectHighlight(loginPnl, 500).Render();
    }
}

using System;
namespace Stebs.View
{
    public interface IOutputWindow
    {
        void Clear();
        void InitializeComponent();
        void WriteError(string text);
        void WriteOutput(string text);
        void WriteSuccess(string text);
    }
}

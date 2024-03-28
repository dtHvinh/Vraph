using HamiltonVisualizer.Constants;
using System.Windows;
using System.Windows.Media.Animation;

namespace HamiltonVisualizer.Core
{
    public class AnimationManager
    {
        public Thickness ModeButtonOn;
        public Thickness ModeButtonOff;

        public Storyboard? StoryboardWhenOn;
        public Storyboard? StoryboardWhenOff;

        private ThicknessAnimation? ThicknessAnimationWhenOn;
        private ThicknessAnimation? ThicknessAnimationWhenOff;

        private static AnimationManager? _instance;
        public static AnimationManager Instance
        {
            get
            {
                _instance ??= new();
                return _instance;
            }
        }

        private AnimationManager()
        {
            InitializeThicknessObjects();
            InitializeThicknessAnimations();
            InitalizeStoryBoards();
        }

        #region Initialize methods

        private void InitializeThicknessAnimations()
        {
            ThicknessAnimationWhenOn = new()
            {
                From = ModeButtonOff,
                To = ModeButtonOn,
                Duration = TimeSpan.FromSeconds(0.3),
            };

            ThicknessAnimationWhenOff = new()
            {
                From = ModeButtonOn,
                To = ModeButtonOff,
                Duration = TimeSpan.FromSeconds(0.3),
            };

        }

        private void InitalizeStoryBoards()
        {
            StoryboardWhenOn = new();
            StoryboardWhenOn.Children.Add(ThicknessAnimationWhenOn);
            Storyboard.SetTargetProperty(ThicknessAnimationWhenOn, new PropertyPath(FrameworkElement.MarginProperty));

            StoryboardWhenOff = new();
            StoryboardWhenOff.Children.Add(ThicknessAnimationWhenOff);
            Storyboard.SetTargetProperty(ThicknessAnimationWhenOff, new PropertyPath(FrameworkElement.MarginProperty));
        }

        private void InitializeThicknessObjects()
        {
            ModeButtonOff = new Thickness(
                ConstantValues.Control.ModeButtonMarginDefault.Item1,
                ConstantValues.Control.ModeButtonMarginDefault.Item2,
                ConstantValues.Control.ModeButtonMarginDefault.Item3,
                ConstantValues.Control.ModeButtonMarginDefault.Item4);

            ModeButtonOn = new Thickness(
                ConstantValues.Control.ModeButtonOn.Item1,
                ConstantValues.Control.ModeButtonOn.Item2,
                ConstantValues.Control.ModeButtonOn.Item3,
                ConstantValues.Control.ModeButtonOn.Item4);
        }

        #endregion Initialize methods
    }
}

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
                ControlConstants.ModeButtonMarginDefault.Item1,
                ControlConstants.ModeButtonMarginDefault.Item2,
                ControlConstants.ModeButtonMarginDefault.Item3,
                ControlConstants.ModeButtonMarginDefault.Item4);

            ModeButtonOn = new Thickness(
                ControlConstants.ModeButtonOn.Item1,
                ControlConstants.ModeButtonOn.Item2,
                ControlConstants.ModeButtonOn.Item3,
                ControlConstants.ModeButtonOn.Item4);
        }

        #endregion Initialize methods
    }
}

﻿using Windows.UI.Xaml;
using Jellyfin.Utils;

namespace Jellyfin.Controls
{
    public class DeviceFamilyStateTrigger : StateTriggerBase
    {
        public static readonly DependencyProperty TargetDeviceFamilyProperty = DependencyProperty.Register(
            "TargetDeviceFamily", typeof(DeviceFormFactorType), typeof(DeviceFamilyStateTrigger), new PropertyMetadata(default(DeviceFormFactorType), OnDeviceTypePropertyChanged));

        public DeviceFormFactorType TargetDeviceFamily
        {
            get { return (DeviceFormFactorType)GetValue(TargetDeviceFamilyProperty); }
            set { SetValue(TargetDeviceFamilyProperty, value); }
        }

        private static void OnDeviceTypePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            var trigger = (DeviceFamilyStateTrigger)dependencyObject;
            var newTargetDeviceFamily = (DeviceFormFactorType)eventArgs.NewValue;
            trigger.SetActive(newTargetDeviceFamily == AppUtils.GetDeviceFormFactorType());
        }
    }
}

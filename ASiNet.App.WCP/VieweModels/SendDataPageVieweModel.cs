using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASiNet.App.WCP.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ASiNet.App.WCP.VieweModels;
public class SendDataPageVieweModel : ObservableObject
{

    public SendFrom[] SendFrom { get; } = 
        [
        new SendFrom() { Flag = Models.Enums.SendFromFlag.File, Name = "Send file" },
        new SendFrom() { Flag = Models.Enums.SendFromFlag.Image, Name = "Send image" },
        new SendFrom() { Flag = Models.Enums.SendFromFlag.Text, Name = "Send text" }
        ];

}

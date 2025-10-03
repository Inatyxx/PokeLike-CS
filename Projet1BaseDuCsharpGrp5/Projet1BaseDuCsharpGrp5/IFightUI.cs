using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet1BaseDuCsharpGrp5
{
    public interface IFightUI
    {
        void Update(Pokemon player, Pokemon enemy);

        void ShowMessage(string message);

        int ChooseFromList(string title, List<string> options);

    }
}

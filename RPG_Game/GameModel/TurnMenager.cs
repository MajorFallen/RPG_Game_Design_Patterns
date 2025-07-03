using ProOb_RPG.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProOb_RPG.GameModel
{
    public interface ITurnObserver
    {
        void UpdateTurn(ITurnMenager turnMenager);
    }
    public interface ITurnMenager
    {
        void AttachTurnObserver(ITurnObserver turnObserver);
        void DetachTurnObserver(ITurnObserver turnObserver);
        void NotifyAboutNextTurn();
    }
    internal class TurnMenager : ITurnMenager
    {
        private bool _isWorking = false;
        public int _turn = 0;
        private List<ITurnObserver> _observers = new List<ITurnObserver>();
        private List<ITurnObserver> _toDetach = new List<ITurnObserver>();

        public void NextTurn()
        {
            _turn++;
            NotifyAboutNextTurn();
        }

        public void AttachTurnObserver(ITurnObserver turnObserver)
        {
            _observers.Add(turnObserver);
        }

        public void DetachTurnObserver(ITurnObserver turnObserver)
        {
            if (!_isWorking)
                _observers.Remove(turnObserver);
            else
                _toDetach.Add(turnObserver);
        }

        public void NotifyAboutNextTurn()
        {
            _isWorking = true;
            foreach (ITurnObserver turnObserver in _observers)
            {
                turnObserver.UpdateTurn(this);
            }
            _isWorking = false;
            foreach (ITurnObserver turnObserver in _toDetach)
            {
                DetachTurnObserver(turnObserver);
            }
            _toDetach.Clear();
        }
    }


}
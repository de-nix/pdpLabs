using System;
using System.Collections.Generic;
using MPI;

namespace lab8_DSM
{
    [Serializable]
    class Msg
    {
        public bool exit = false;
        public string var;
        public int rankVal;
        public string type;
        public int newVal;

        public Msg(bool exit)
        {
            this.exit = exit;
        }

        public Msg(string var, int rankval, string type, int newVal= -1)
        {
            this.var = var;
            this.rankVal = rankval;
            this.type = type;
            this.newVal = newVal; 
        }
    }

    class DSM
    {
        public int a = 1, b = 2;
        public Dictionary<String, List<int>> subscribers = new Dictionary<string, List<int>>(); 

        public DSM() {
            subscribers.Add("a", new List<int>());
            subscribers.Add("b", new List<int>());
        }

        public void update(string var, int val)
        {
            this.setVariable(var, val); 
            Msg msg = new Msg(var, val, "update");
            for (int i = 0; i < Communicator.world.Size; i++)
            {
                if (Communicator.world.Rank == i) continue;
                if (!subscribers[var].Contains(i)) continue;
                Communicator.world.Send(msg, i, 0);
            }
        }

        public void close()
        {
            for (int i = 0; i < Communicator.world.Size; i++)
            {
                if (Communicator.world.Rank == i) continue;
                Communicator.world.Send(new Msg(true), i, 0);
            }
        }

        public void setVariable(string var, int val)
        {
            if (var == "a") a = val; 
            if (var == "b") b = val;
        }

        public void subscribe(string var)
        {
            this.subscribers[var].Add(Communicator.world.Rank);
            Msg msg = new Msg(var, Communicator.world.Rank, "subscribe");
            for (int i = 0; i < Communicator.world.Size; i++)
            {
                if (Communicator.world.Rank == i) continue;
                Communicator.world.Send(msg, i, 0);
            }
        }

        public void forwardSubscribe(string var, int rank)
        {
            this.subscribers[var].Add(rank); 
        }

        internal void replace(string var, int val, int newVal)
        {
           if (var == "a" && a==val) update("a", newVal);
           if (var == "b" && b == val) update("b", newVal);
        }
    }
}

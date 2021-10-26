using System;
using System.Collections.Generic;

namespace Solcery.BrickInterpretation
{
    public class BrickService : Service<BrickService>, IBrickService
    {
        private Dictionary<string, Type> _brickTypeNameToType;
        private Dictionary<string, Stack<Brick>> _brickPool;

        public BrickService()
        {
            _brickTypeNameToType = new Dictionary<string, Type>(20);
            _brickPool = new Dictionary<string, Stack<Brick>>(20);
        }

        public void Registration(Brick brick)
        {
            _brickTypeNameToType.Add(brick.BrickTypeName(), brick.GetType());
            _brickPool.Add(brick.BrickTypeName(), new Stack<Brick>(10));
            _brickPool[brick.BrickTypeName()].Push(brick);
        }

        public void Cleanup()
        {
            _brickTypeNameToType.Clear();
            _brickPool.Clear();
        }

        public bool TryCreate<T>(string brickTypeName, out T brick) where T : Brick
        {
            brick = null;

            if (!_brickTypeNameToType.ContainsKey(brickTypeName))
            {
                return false;
            }

            Brick br;

            if (_brickPool.ContainsKey(brickTypeName) && _brickPool[brickTypeName].Count > 0)
            {
                br = _brickPool[brickTypeName].Pop();
            }
            else
            {
                br = (Brick) Activator.CreateInstance(_brickTypeNameToType[brickTypeName]);
            }

            if (br is T brT)
            {
                brick = brT;
                return true;
            }
            
            Free(br);
            return false;
        }

        public void Free(Brick brick)
        {
            if (brick == null)
            {
                return;
            }

            if (!_brickPool.ContainsKey(brick.BrickTypeName()))
            {
                _brickPool[brick.BrickTypeName()] = new Stack<Brick>(10);
            }
            
            brick.Reset();
            _brickPool[brick.BrickTypeName()].Push(brick);
        }
    }
}
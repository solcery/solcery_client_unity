using System;
using System.Collections.Generic;

namespace Solcery.BrickInterpretation
{
    public class BrickService : Service<BrickService>
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

        public Brick Create(string brickTypeName)
        {
            if (!_brickTypeNameToType.ContainsKey(brickTypeName))
            {
                return null;
            }
            
            if (_brickPool.ContainsKey(brickTypeName) && _brickPool[brickTypeName].Count > 0)
            {
                return _brickPool[brickTypeName].Pop();
            }
            
            return (Brick) Activator.CreateInstance(_brickTypeNameToType[brickTypeName]);
        }

        public bool TryCreate(string brickTypeName, out Brick brick)
        {
            brick = Create(brickTypeName);
            return brick != null;
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
using System;
using System.Collections.Generic;

namespace CalculatriceMargeWPF.Models
{
    /// <summary>
    /// Gestionnaire Undo/Redo pour les calculs
    /// </summary>
    public class UndoRedoManager
    {
        private Stack<CalculationEngine.CalculationResult> _undoStack;
        private Stack<CalculationEngine.CalculationResult> _redoStack;

        public event EventHandler UndoRedoChanged;

        public UndoRedoManager()
        {
            _undoStack = new Stack<CalculationEngine.CalculationResult>();
            _redoStack = new Stack<CalculationEngine.CalculationResult>();
        }

        public bool CanUndo => _undoStack.Count > 0;
        public bool CanRedo => _redoStack.Count > 0;

        public void Push(CalculationEngine.CalculationResult result)
        {
            _undoStack.Push(result);
            _redoStack.Clear();
            UndoRedoChanged?.Invoke(this, EventArgs.Empty);
        }

        public CalculationEngine.CalculationResult Undo()
        {
            if (!CanUndo) return null;
            var result = _undoStack.Pop();
            _redoStack.Push(result);
            UndoRedoChanged?.Invoke(this, EventArgs.Empty);
            return result;
        }

        public CalculationEngine.CalculationResult Redo()
        {
            if (!CanRedo) return null;
            var result = _redoStack.Pop();
            _undoStack.Push(result);
            UndoRedoChanged?.Invoke(this, EventArgs.Empty);
            return result;
        }

        public void Clear()
        {
            _undoStack.Clear();
            _redoStack.Clear();
            UndoRedoChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}

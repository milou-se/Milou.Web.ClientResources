using System;

namespace Milou.Web.ClientResources
{
    public static class GlobalVersion
    {
        private static IGlobalVersionCreator _globalVersionCreator;

        private static volatile string _current;

        private static readonly object _IdCreatorMutexLock = new object();

        private static readonly object _IdMutexLock = new object();

        private static UpdateMode _updateMode;

        public static string Current
        {
            get
            {
                if (!IsInitialized())
                {
                    throw new InvalidOperationException(
                        $"The global version id has noot been initialized, ensure to call {nameof(Initialize)} method first");
                }

                return _current;
            }
        }

        public static void Update()
        {
            if (!IsInitialized())
            {
                throw new InvalidOperationException("The global version id is not initialized");
            }

            if (_updateMode == UpdateMode.Deny)
            {
                throw new InvalidOperationException(
                    $"The global version id could only be updated if {nameof(UpdateMode)} is set to {nameof(UpdateMode.Allow)}");
            }

            InternalUpdate();
        }

        public static void Initialize(
            IGlobalVersionCreator globalVersionCreator,
            UpdateMode updateMode = UpdateMode.Deny)
        {
            if (globalVersionCreator == null)
            {
                throw new ArgumentNullException(nameof(globalVersionCreator));
            }

            if (_globalVersionCreator != null)
            {
                throw new InvalidOperationException(
                    "The global version id could only be initialized once (first check)");
            }

            lock (_IdCreatorMutexLock)
            {
                if (_globalVersionCreator != null)
                {
                    throw new InvalidOperationException(
                        "The global version id could only be initialized once (second check, in mutex lock)");
                }

                _globalVersionCreator = globalVersionCreator;
                _updateMode = updateMode;

                InternalUpdate();
            }
        }

        public static bool IsInitialized()
        {
            return _globalVersionCreator != null;
        }

        private static void InternalUpdate()
        {
            lock (_IdMutexLock)
            {
                _current = _globalVersionCreator.CreateGlobalId();
            }
        }
    }
}
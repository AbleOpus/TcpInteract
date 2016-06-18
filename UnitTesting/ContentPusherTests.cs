using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TcpInteract;

namespace UnitTesting
{
    [TestClass]
    public class ContentPusherTests
    {
        /// <summary>
        /// Checks to see if the <see cref="ContentPusher"/> can do a basic bind
        /// and push correctly.
        /// </summary>
        [TestMethod]
        public void BasicBindPush_Assert()
        {
            ContentPusher pusher = new ContentPusher();
            bool fired = false;
            int val = 0;

            pusher.Bind<Animal>(o =>
            {
                fired = true;
                val = o.TestProp1;
            });

            pusher.Push(new Animal {TestProp1 = 3 });

           Assert.IsTrue(fired && val == 3);
        }

        /// <summary>
        /// Types should be exact, a cat handler should not handle an Animal.
        /// </summary>
        [TestMethod]
        public void Deritive_Throw()
        {
            ContentPusher pusher = new ContentPusher();
            pusher.Bind<Cat>(o => { Assert.Fail();});
            pusher.Push(new Animal { TestProp1 = 3 });
        }

        /// <summary>
        /// Check to see if methods unbind properly.
        /// </summary>
        [TestMethod]
        public void Unbind_Throw()
        {
            Action<Cat> method = o => Assert.Fail();
            ContentPusher pusher = new ContentPusher();
            pusher.Bind(method);
            pusher.Unbind(method);
            pusher.Push(new Cat());
        }

        /// <summary>
        /// More than one method should be able to subscribe to a type.
        /// </summary>
        [TestMethod]
        public void MultipleSubscribers_Assert()
        {
            bool a1Raised = false;
            bool a2Raised = false;
            Action<Cat> action1 = delegate { a1Raised = true; };
            Action<Cat> action2 = delegate { a2Raised = true; };
            ContentPusher pusher = new ContentPusher();
            pusher.Bind(action1);
            pusher.Bind(action2);
            pusher.Push(new Cat());
            Assert.IsTrue(a1Raised && a2Raised);
        }

        /// <summary>
        /// Make sure unbind, unbinds by delegate and not type.
        /// </summary>
        [TestMethod]
        public void MultipleSubscribersUnbind_Assert()
        {
            bool a1Raised = false;
            Action<Cat> action1 = delegate { a1Raised = true; };
            Action<Cat> action2 = delegate { };
            ContentPusher pusher = new ContentPusher();
            pusher.Bind(action1);
            pusher.Bind(action2);
            pusher.Unbind(action2);
            pusher.Push(new Cat());
            Assert.IsTrue(a1Raised);
        }

        /// <summary>
        /// Binding the same method twice should raise an exception.
        /// </summary>
        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void RedundantBind_Throw()
        {
            Action<Cat> method = o => Assert.Fail();
            ContentPusher pusher = new ContentPusher();
            pusher.Bind(method);
            pusher.Bind(method);
        }

        private class Animal
        {
            public int TestProp1 { get; set; }
        }

        private class Cat : Animal {}
    }
}

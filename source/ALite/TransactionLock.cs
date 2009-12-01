using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;
using System.Threading;

namespace ALite
{
	public class TransactionLock
	{
		LinkedList<KeyValuePair<Transaction, ManualResetEvent>> mTransactionQueue = new LinkedList<KeyValuePair<Transaction, ManualResetEvent>>();
		Transaction mCurrentTransaction;

		/// <summary>
		/// The currently-running transaction.
		/// </summary>
		public Transaction CurrentTransaction
		{
			get
			{
				lock (this)
				{
					return mCurrentTransaction;
				}
			}
			private set
			{
				lock (this)
				{
					mCurrentTransaction = value;
				}
			}
		}

		/// <summary>
		/// Acquires the lock for the current transaction.  Blocks if another transaction has already acquired the lock.
		/// </summary>
		public void AcquireLock()
		{
			AcquireLock(Transaction.Current);
		}

		/// <summary>
		/// Acquires the lock for the specified transaction.  Blocks if another transaction has already acquired the lock.
		/// </summary>
		/// <param name="transaction"></param>
		void AcquireLock(Transaction transaction)
		{
			// Abort trying to lock if there is no transaction to receive the lock
			if (transaction == null) return;

			// Owning transaction trying to reacquire lock
			if (mCurrentTransaction == transaction) return;

			// If not locked, acquire the lock
			if (mCurrentTransaction == null)
			{
				mCurrentTransaction = transaction;
				return;
			}

			// Need to queue the transaction until the lock is released
			QueueTransaction(transaction);
		}

		/// <summary>
		/// Enqueues and blocks the transaction.
		/// </summary>
		/// <param name="transaction">The transaction to enqueue.</param>
		private void QueueTransaction(Transaction transaction)
		{

			ManualResetEvent manualEvent = new ManualResetEvent(false);

			KeyValuePair<Transaction, ManualResetEvent> pair;
			pair = new KeyValuePair<Transaction, ManualResetEvent>(transaction, manualEvent);

			lock (this)
			{
				mTransactionQueue.AddLast(pair);
			}

			// Ensure that the transaction removes itself from the queue when it completes
			transaction.TransactionCompleted += delegate
			{
				lock (this)
				{
					mTransactionQueue.Remove(pair);
				}
				lock (manualEvent)
				{
					if (manualEvent.SafeWaitHandle.IsClosed == false)
					{
						manualEvent.Set();
					}
				}
			};

			// Block until the transaction is signalled
			manualEvent.WaitOne();
			lock (manualEvent)
			{
				manualEvent.Close();
			}
		}

		/// <summary>
		/// Release the lock.  If there are any queued transactions, unblock the first in the queue.
		/// </summary>
		public void ReleaseLock()
		{
			mCurrentTransaction = null;

			LinkedListNode<KeyValuePair<Transaction, ManualResetEvent>> node = DequeueTransaction();

			if (node != null)
			{
				Transaction transaction = node.Value.Key;
				ManualResetEvent manualEvent = node.Value.Value;

				AcquireLock(transaction);

				lock (manualEvent)
				{
					if (manualEvent.SafeWaitHandle.IsClosed == false)
					{
						manualEvent.Set();
					}
				}
			}
		}

		/// <summary>
		/// Dequeue and return the first node in the queue.
		/// </summary>
		/// <returns>The first node in the queue if one is available.</returns>
		private LinkedListNode<KeyValuePair<Transaction, ManualResetEvent>> DequeueTransaction()
		{
			LinkedListNode<KeyValuePair<Transaction, ManualResetEvent>> node = null;

			lock (this)
			{
				if (mTransactionQueue.Count > 0)
				{
					node = mTransactionQueue.First;
					mTransactionQueue.RemoveFirst();
				}
			}

			return node;
		}
	}
}

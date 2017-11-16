using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace TelnetServer.Messages {
	class FlatFileDB {

		ulong IDSeed = 0;
		object dictLocker = new object();
		Dictionary<string, ulong> files;
		Dictionary<ulong, int> openedFiles;
		Dictionary<ulong, ReaderWriterLock> fileLocker;
		
		int timeout = 2000;

		private FlatFileDB() {
			this.files = new Dictionary<string, ulong>();
			this.openedFiles = new Dictionary<ulong, int>();
			this.fileLocker = new Dictionary<ulong, ReaderWriterLock>();
		}

		public T GetObject<T>(string filename) where T : ISerializable {
			if (File.Exists(filename)) {

				ulong id;
				ReaderWriterLock wrlock;

				lock (dictLocker) {
					if (!files.ContainsKey(filename)) {
						id = ++IDSeed;
						files.Add(filename, id);
						openedFiles.Add(id, 1);
						wrlock = new ReaderWriterLock();
						fileLocker.Add(id, wrlock);
					} else {
						id = files[filename];
						openedFiles[id] += 1;
						wrlock = fileLocker[id];
					}

				}

				wrlock.AcquireReaderLock(timeout);
				try {
					using (FileStream fs = File.Open(filename, FileMode.Open, FileAccess.Read)) {
						BinaryFormatter formatter = new BinaryFormatter();
						object obj = formatter.Deserialize(fs);
						return (obj is T) ? (T)obj : default(T);
					}
				} finally {
					wrlock.ReleaseReaderLock();
					lock (dictLocker) {
						openedFiles[id] -= 1;
						if (openedFiles[id] == 0) {
							openedFiles.Remove(id);
							fileLocker.Remove(id);
							files.Remove(filename);
						}
					}
				}
			}

			return default(T);
		}

		public void PercistObject<T>(string filename, T data) where T : ISerializable {

			if (File.Exists(filename)) {

				ulong id;
				ReaderWriterLock wrlock;

				lock (dictLocker) {
					if (!files.ContainsKey(filename)) {
						id = ++IDSeed;
						files.Add(filename, id);
						openedFiles.Add(id, 1);
						wrlock = new ReaderWriterLock();
						fileLocker.Add(id, wrlock);
					} else {
						id = files[filename];
						openedFiles[id] += 1;
						wrlock = fileLocker[id];
					}

				}

				wrlock.AcquireWriterLock(timeout);
				try {
					using (FileStream fs = File.Open(filename, FileMode.Create, FileAccess.ReadWrite)) {
						BinaryFormatter formatter = new BinaryFormatter();
						formatter.Serialize(fs, data);			
					}
				} finally {
					wrlock.ReleaseWriterLock();
					lock (dictLocker) {
						openedFiles[id] -= 1;
						if (openedFiles[id] == 0) {
							openedFiles.Remove(id);
							fileLocker.Remove(id);
							files.Remove(filename);
						}
					}
				}
			}
		}

		public void WriteFile(string filename, string data) {

			if (File.Exists(filename)) {

				ulong id;
				ReaderWriterLock wrlock;

				lock (dictLocker) {
					if (!files.ContainsKey(filename)) {
						id = ++IDSeed;
						files.Add(filename, id);
						openedFiles.Add(id, 1);
						wrlock = new ReaderWriterLock();
						fileLocker.Add(id, wrlock);
					} else {
						id = files[filename];
						openedFiles[id] += 1;
						wrlock = fileLocker[id];
					}

				}

				wrlock.AcquireWriterLock(timeout);
				try {
					using (StreamWriter sr = new StreamWriter(File.Open(filename, FileMode.Create, FileAccess.ReadWrite))) {
						sr.Write(data);
					}
				} finally {
					wrlock.ReleaseWriterLock();
					lock (dictLocker) {
						openedFiles[id] -= 1;
						if (openedFiles[id] == 0) {
							openedFiles.Remove(id);
							fileLocker.Remove(id);
							files.Remove(filename);
						}
					}
				}
			}
			
		}

		public string ReadFile(string filename) {

			if (File.Exists(filename)) {

				ulong id;
				ReaderWriterLock wrlock;

				lock (dictLocker) {
					if (!files.ContainsKey(filename)) {
						id = ++IDSeed;
						files.Add(filename, id);
						openedFiles.Add(id, 1);
						wrlock = new ReaderWriterLock();
						fileLocker.Add(id, wrlock);
					} else {
						id = files[filename];
						openedFiles[id] += 1;
						wrlock = fileLocker[id];
					}

				}

				wrlock.AcquireReaderLock(timeout);
				try {
					using (StreamReader sr = new StreamReader(File.Open(filename, FileMode.Open, FileAccess.Read))) {
						return sr.ReadToEnd();
					}
				} finally {
					wrlock.ReleaseReaderLock();
					lock (dictLocker) {
						openedFiles[id] -= 1;
						if (openedFiles[id] == 0) {
							openedFiles.Remove(id);
							fileLocker.Remove(id);
							files.Remove(filename);
						}
					}
				}
			}

			return string.Empty;
		}

		private static FlatFileDB instance;
		private static object instanceLocker = new object();
		public static FlatFileDB Instance {
			get {
				if (instance == null) {
					lock (instanceLocker) {
						if (instance == null) {
							instance = new FlatFileDB();
						}
					}
				}
				return instance;
			}

		}


	}
}

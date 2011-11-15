#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Willcraftia.Xna.Foundation.Graphics;

#endregion

namespace Willcraftia.Xna.Foundation.Debugs
{
    /// <summary>
    /// ITimeRuler を実装する DebugComponent です。
    /// </summary>
    public sealed class TimeRulerComponent : DebugComponent, ITimeRuler
    {
        #region Inner classes

        /// <summary>
        /// マーカー構造体
        /// </summary>
        struct Marker
        {
            public int MarkerId;
            public float BeginTime;
            public float EndTime;
            public Color Color;
        }

        /// <summary>
        /// マーカーコレクション
        /// </summary>
        class MarkerCollection
        {
            // マーカーコレクション
            public Marker[] Markers = new Marker[MaxSamples];
            public int MarkCount;

            // マーカーネスト情報
            public int[] MarkerNests = new int[MaxNestCall];
            public int NestCount;
        }

        /// <summary>
        /// フレームのログ
        /// </summary>
        class FrameLog
        {
            // バー情報
            public MarkerCollection[] Bars;

            public FrameLog()
            {
                // マーカーコレクション配列の初期化
                Bars = new MarkerCollection[MaxBars];
                for (int i = 0; i < MaxBars; ++i)
                    Bars[i] = new MarkerCollection();
            }
        }

        /// <summary>
        /// マーカー情報
        /// </summary>
        class MarkerInfo
        {
            // マーカー名
            public string Name;

            // マーカーログ
            public MarkerLog[] Logs = new MarkerLog[MaxBars];

            public MarkerInfo(string name)
            {
                Name = name;
            }
        }

        /// <summary>
        /// マーカーログ情報
        /// </summary>
        struct MarkerLog
        {
            public float SnapMin;
            public float SnapMax;
            public float SnapAvg;

            public float Min;
            public float Max;
            public float Avg;

            public int Samples;

            public bool Initialized;
        }

        #endregion

        #region Fields and Properties

        /// <summary>
        /// 最大バー表示数
        /// </summary>
        const int MaxBars = 10;

        /// <summary>
        /// バーひとつあたりの最大サンプル数
        /// </summary>
        const int MaxSamples = 256;

        /// <summary>
        /// バーひとつあたりの最大ネスト数
        /// </summary>
        const int MaxNestCall = 32;

        /// <summary>
        /// 最多表示フレーム数
        /// </summary>
        const int MaxSampleFrames = 4;

        /// <summary>
        /// ログのスナップを取る間隔(フレーム数)
        /// </summary>
        const int LogSnapDuration = 120;

        /// <summary>
        /// バーの高さ(ピクセル)
        /// </summary>
        const int BarHeight = 2;

        /// <summary>
        /// バーのパディング(ピクセル)
        /// </summary>
        const int BarPadding = 2;

        /// <summary>
        /// 自動表示フレーム調整に掛かるフレーム数
        /// </summary>
        const int AutoAdjustDelay = 30;

        Color windowColor = new Color(0, 0, 0, 128);

        /// <summary>
        /// ウィンドウ背景色を取得または設定します。
        /// </summary>
        public Color WindowColor
        {
            get { return windowColor; }
            set { windowColor = value; }
        }

        Color fontColor = Color.White;

        /// <summary>
        /// 文字色を取得または設定します。
        /// </summary>
        public Color FontColor
        {
            get { return fontColor; }
            set { fontColor = value; }
        }

        Color frameGridColor = Color.White;

        /// <summary>
        /// フレーム単位グリッドの色を取得または設定します。
        /// </summary>
        public Color FrameGridColor
        {
            get { return frameGridColor; }
            set { frameGridColor = value; }
        }

        Color millisecondGridColor = Color.Gray;

        /// <summary>
        /// ミリ秒単位グリッドの色を取得または設定します。
        /// </summary>
        public Color MillisecondGridColor
        {
            get { return millisecondGridColor; }
            set { millisecondGridColor = value; }
        }

        int width;

        /// <summary>
        /// 描画幅を取得または設定します。
        /// </summary>
        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        // フレーム毎のログ
        FrameLog[] logs;

        // 前フレームのログ
        FrameLog prevLog;

        // 測定中のフレームログ
        FrameLog curLog;

        // 現在のフレーム数
        int frameCount;

        // 計測に使用するストップウォッチ
        Stopwatch stopwatch = new Stopwatch();

        // マーカ情報配列
        List<MarkerInfo> markers = new List<MarkerInfo>();

        // マーカ名からマーカ ID への変換マップ
        Dictionary<string, int> markerNameToIdMap = new Dictionary<string, int>();

        // サンプルフレーム自動調整用のカウンタ
        int frameAdjust;

        // 現在の表示フレーム数
        int sampleFrames;

        // マーカ ログ表示文字列
        StringBuilder logString = new StringBuilder(512);

        // TimerRuler の表示位置
        Vector2 position;

        #endregion

        #region Constructors

        /// <summary>
        /// インスタンスを生成します。
        /// </summary>
        /// <param name="game">インスタンスを登録する Game。</param>
        public TimeRulerComponent(Game game)
            : base(game)
        {
            game.Components.ComponentAdded += new EventHandler<GameComponentCollectionEventArgs>(OnComponentAdded);
            game.Components.ComponentRemoved += new EventHandler<GameComponentCollectionEventArgs>(OnComponentRemoved);
        }

        /// <summary>
        /// 自分自身が GameComponent として登録される時に、自分自身を現在有効な ITimeRuler として設定します。
        /// </summary>
        /// <param name="sender">イベントのソース。</param>
        /// <param name="e">イベント データ。</param>
        void OnComponentAdded(object sender, GameComponentCollectionEventArgs e)
        {
            if (e.GameComponent == this)
            {
                TimeRuler.Instance = this;
            }
        }

        /// <summary>
        /// 自分自身が GameComponent としての登録から解除される時に、
        /// 自分自身が現在有効な ITimeRuler ではなくなるように設定します。
        /// </summary>
        /// <param name="sender">イベントのソース。</param>
        /// <param name="e">イベント データ。</param>
        void OnComponentRemoved(object sender, GameComponentCollectionEventArgs e)
        {
            if (e.GameComponent == this)
            {
                TimeRuler.Instance = null;
            }
        }

        #endregion

        protected override void LoadContent()
        {
            logs = new FrameLog[2];
            for (int i = 0; i < logs.Length; ++i)
            {
                logs[i] = new FrameLog();
            }

            sampleFrames = targetSampleFrames = 1;

            var vp = GraphicsDevice.Viewport;
            var layout = new Layout(new Rectangle(vp.X, vp.Y, vp.Width, vp.Height), vp.TitleSafeArea);

            width = (int) (layout.ClientArea.Width * 0.8f);
            
            position = layout.Place(
                new Vector2(width, BarHeight),
                0.0f,
                0.01f,
                LayoutAlignment.BottomCenter);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            StartFrame();
        }

        public override void Draw(GameTime gameTime)
        {
            Draw(position, width);
        }

        #region ITimeRuler

        bool logVisible = true;
        public bool LogVisible
        {
            get { return logVisible; }
            set { logVisible = value; }
        }

        bool barVisible = true;
        public bool BarVisible
        {
            get { return barVisible; }
            set { barVisible = value; }
        }

        int targetSampleFrames;
        public int TargetSampleFrames
        {
            get { return targetSampleFrames; }
            set { targetSampleFrames = value; }
        }

        public void BeginMark(string markerName, Color color)
        {
            BeginMark(markerName, color, 0);
        }

        public void BeginMark(string markerName, Color color, int barIndex)
        {
            lock (this)
            {
                if (barIndex < 0 || barIndex >= MaxBars)
                {
                    throw new ArgumentOutOfRangeException("barIndex");
                }

                var bar = curLog.Bars[barIndex];

                if (bar.MarkCount >= MaxSamples)
                {
                    throw new OverflowException(
                        "サンプル数がMaxSampleを超えました。\n" +
                        "TimeRuler.MaxSmpaleの値を大きくするか、" +
                        "サンプル数を少なくしてください。");
                }

                if (bar.NestCount >= MaxNestCall)
                {
                    throw new OverflowException(
                        "ネスト数がMaxNestCallを超えました。\n" +
                        "TimeRuler.MaxNestCallの値を大きくするか、" +
                        "ネスト呼び出し数を減らしてください。");
                }

                // 登録されているマーカーを取得
                int markerId;
                if (!markerNameToIdMap.TryGetValue(markerName, out markerId))
                {
                    // 登録されていなければ新たに登録する
                    markerId = markers.Count;
                    markerNameToIdMap.Add(markerName, markerId);
                    markers.Add(new MarkerInfo(markerName));
                }

                // 測定開始
                bar.MarkerNests[bar.NestCount++] = bar.MarkCount;

                // マーカーのパラメーターを設定
                bar.Markers[bar.MarkCount].MarkerId = markerId;
                bar.Markers[bar.MarkCount].Color = color;
                bar.Markers[bar.MarkCount].BeginTime = (float) stopwatch.Elapsed.TotalMilliseconds;

                bar.Markers[bar.MarkCount].EndTime = -1;

                bar.MarkCount++;
            }
        }

        public void EndMark(string markerName)
        {
            EndMark(markerName, 0);
        }

        public void EndMark(string markerName, int barIndex)
        {
            lock (this)
            {
                if (barIndex < 0 || barIndex >= MaxBars)
                {
                    throw new ArgumentOutOfRangeException("barIndex");
                }

                var bar = curLog.Bars[barIndex];

                if (bar.NestCount <= 0)
                {
                    throw new InvalidOperationException("Please call BeginMark method before EndMark one");
                }

                int markerId;
                if (!markerNameToIdMap.TryGetValue(markerName, out markerId))
                {
                    throw new InvalidOperationException(
                        String.Format("Maker name '{0}' not registered", markerName));
                }

                var markerIdx = bar.MarkerNests[--bar.NestCount];
                if (bar.Markers[markerIdx].MarkerId != markerId)
                {
                    throw new InvalidOperationException("Invalid BeginMark/EndMark call sequence");
                }

                bar.Markers[markerIdx].EndTime = (float) stopwatch.Elapsed.TotalMilliseconds;
            }
        }

        public void ResetLog()
        {
            lock (this)
            {
                foreach (var markerInfo in markers)
                {
                    for (int i = 0; i < markerInfo.Logs.Length; ++i)
                    {
                        markerInfo.Logs[i].Initialized = false;
                        markerInfo.Logs[i].SnapMin = 0;
                        markerInfo.Logs[i].SnapMax = 0;
                        markerInfo.Logs[i].SnapAvg = 0;

                        markerInfo.Logs[i].Min = 0;
                        markerInfo.Logs[i].Max = 0;
                        markerInfo.Logs[i].Avg = 0;

                        markerInfo.Logs[i].Samples = 0;
                    }
                }
            }
        }

        #endregion

        void StartFrame()
        {
            lock (this)
            {
                // 現在のフレームログの更新
                prevLog = logs[frameCount++ & 0x1];
                curLog = logs[frameCount & 0x1];

                var endFrameTime = (float) stopwatch.Elapsed.TotalMilliseconds;

                // マーカーの更新とログ生成
                for (int barIdx = 0; barIdx < prevLog.Bars.Length; ++barIdx)
                {
                    var prevBar = prevLog.Bars[barIdx];
                    var nextBar = curLog.Bars[barIdx];

                    // 前フレームでEndMarkを呼んでいないマーカーを閉じ、現フレームで
                    // 再度開く。
                    for (int nest = 0; nest < prevBar.NestCount; ++nest)
                    {
                        int markerIdx = prevBar.MarkerNests[nest];

                        prevBar.Markers[markerIdx].EndTime = endFrameTime;

                        nextBar.MarkerNests[nest] = nest;
                        nextBar.Markers[nest].MarkerId = prevBar.Markers[markerIdx].MarkerId;
                        nextBar.Markers[nest].BeginTime = 0;
                        nextBar.Markers[nest].EndTime = -1;
                        nextBar.Markers[nest].Color = prevBar.Markers[markerIdx].Color;
                    }

                    // マーカーログの更新
                    for (int markerIdx = 0; markerIdx < prevBar.MarkCount; ++markerIdx)
                    {
                        var duration = prevBar.Markers[markerIdx].EndTime - prevBar.Markers[markerIdx].BeginTime;

                        var markerId = prevBar.Markers[markerIdx].MarkerId;
                        var m = markers[markerId];

                        if (!m.Logs[barIdx].Initialized)
                        {
                            // 最初のフレームの処理
                            m.Logs[barIdx].Min = duration;
                            m.Logs[barIdx].Max = duration;
                            m.Logs[barIdx].Avg = duration;

                            m.Logs[barIdx].Initialized = true;
                        }
                        else
                        {
                            // ２フレーム目以降の処理
                            m.Logs[barIdx].Min = Math.Min(m.Logs[barIdx].Min, duration);
                            m.Logs[barIdx].Max = Math.Min(m.Logs[barIdx].Max, duration);
                            m.Logs[barIdx].Avg += duration;
                            m.Logs[barIdx].Avg *= 0.5f;

                            if (m.Logs[barIdx].Samples++ >= LogSnapDuration)
                            {
                                m.Logs[barIdx].SnapMin = m.Logs[barIdx].Min;
                                m.Logs[barIdx].SnapMax = m.Logs[barIdx].Max;
                                m.Logs[barIdx].SnapAvg = m.Logs[barIdx].Avg;
                                m.Logs[barIdx].Samples = 0;
                            }
                        }
                    }

                    nextBar.MarkCount = prevBar.NestCount;
                    nextBar.NestCount = prevBar.NestCount;
                }

                // このフレームの測定開始
                stopwatch.Reset();
                stopwatch.Start();
            }
        }

        void Draw(Vector2 position, int width)
        {
            // 表示するべきバーの数によって表示サイズと位置を変更する
            int height = 0;
            float maxTime = 0;
            foreach (var bar in prevLog.Bars)
            {
                if (barVisible && bar.MarkCount > 0)
                {
                    height += BarHeight + BarPadding * 2;
                    maxTime = Math.Max(maxTime, bar.Markers[bar.MarkCount - 1].EndTime);
                }
            }

            // 表示フレーム数の自動調整
            // 例えば 16.6ms で処理が間に合わなかった状態が一定時間以上続くと
            // 自動的に表示する時間間隔を 33.3ms に調整する
            const float frameSpan = 1.0f / 60.0f * 1000f;
            var sampleSpan = (float) sampleFrames * frameSpan;

            if (maxTime > sampleSpan)
            {
                frameAdjust = Math.Max(0, frameAdjust) + 1;
            }
            else
            {
                frameAdjust = Math.Min(0, frameAdjust) - 1;
            }

            if (Math.Abs(frameAdjust) > AutoAdjustDelay)
            {
                sampleFrames = Math.Min(MaxSampleFrames, sampleFrames);
                sampleFrames = Math.Max(targetSampleFrames, (int) (maxTime / frameSpan) + 1);

                frameAdjust = 0;
            }

            // ミリ秒からピクセルに変換する係数を計算
            var msToPs = (float) width / sampleSpan;

            // 描画開始位置
            var startY = (int) position.Y - (height - BarHeight);

            // 現在のy座標
            var y = startY;

            SpriteBatch.Begin();
            {
                if (barVisible)
                {
                    // 背景の半透明の矩形を描画
                    var rect = new Rectangle((int) position.X, y, width, height);
                    SpriteBatch.Draw(FillTexture, rect, windowColor);

                    // 各バーのマーカーを描画
                    rect.Height = BarHeight;
                    foreach (var bar in prevLog.Bars)
                    {
                        rect.Y = y + BarPadding;
                        if (bar.MarkCount > 0)
                        {
                            for (int j = 0; j < bar.MarkCount; ++j)
                            {
                                float bt = bar.Markers[j].BeginTime;
                                float et = bar.Markers[j].EndTime;
                                int sx = (int) (position.X + bt * msToPs);
                                int ex = (int) (position.X + et * msToPs);
                                rect.X = sx;
                                rect.Width = Math.Max(ex - sx, 1);

                                SpriteBatch.Draw(FillTexture, rect, bar.Markers[j].Color);
                            }
                        }

                        y += BarHeight + BarPadding;
                    }

                    // グリッドを描画する
                    // ミリ秒単位のグリッド描画
                    rect = new Rectangle((int) position.X, (int) startY, 1, height);

                    for (float t = 1.0f; t < sampleSpan; t += 1.0f)
                    {
                        rect.X = (int) (position.X + t * msToPs);
                        SpriteBatch.Draw(FillTexture, rect, millisecondGridColor);
                    }

                    // フレーム単位のグリッド描画
                    for (int i = 0; i <= sampleFrames; ++i)
                    {
                        rect.X = (int) (position.X + frameSpan * (float) i * msToPs);
                        SpriteBatch.Draw(FillTexture, rect, frameGridColor);
                    }
                }

                // ログの表示
                if (logVisible)
                {
                    // 表示する文字列をStringBuilderで生成する
                    y = startY - Font.LineSpacing;
                    logString.Length = 0;
                    foreach (var markerInfo in markers)
                    {
                        for (int i = 0; i < MaxBars; ++i)
                        {
                            if (markerInfo.Logs[i].Initialized)
                            {
                                if (logString.Length > 0)
                                {
                                    logString.Append("\n");
                                }

                                logString.Append(" Bar ");
                                logString.AppendNumber(i);
                                logString.Append(" ");
                                logString.Append(markerInfo.Name);

                                logString.Append(" Avg.:");
                                logString.AppendNumber(markerInfo.Logs[i].SnapAvg);
                                logString.Append("ms ");

                                y -= Font.LineSpacing;
                            }
                        }
                    }

                    // 表示する文字列の背景の矩形サイズの計算と描画
                    var size = Font.MeasureString(logString);
                    var rect = new Rectangle((int) position.X, (int) y, (int) size.X, (int) size.Y);
                    SpriteBatch.Draw(FillTexture, rect, windowColor);

                    // ログ文字列の描画
                    SpriteBatch.DrawString(Font, logString, new Vector2(position.X, y), fontColor);
                }
            }
            SpriteBatch.End();
        }
    }
}

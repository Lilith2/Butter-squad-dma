using SkiaSharp;

namespace squad_dma
{
    /// <summary>
    /// Extension methods go here.
    /// </summary>
    public static class Extensions
    {
        private static SKPaint textOutlinePaint = null;
        private static SKPaint projectilePaint = null;
        private static Dictionary<Team, SKPaint> teamEntityPaints = [];
        private static Dictionary<Team, SKPaint> teamTextPaints = [];
        private static readonly Dictionary<Team, SKColor> TeamColors = new(){
            {Team.RU, new SKColor(34, 139, 34)},           // Forest Green, brighter to stand out
            {Team.US, new SKColor(255, 0, 0)},             // Bright Red for USA
            {Team.AU, new SKColor(255, 215, 0)},           // Bright Gold for Australia
            {Team.UK, new SKColor(204, 0, 51)},            // Deep Red with brightness for UK
            {Team.CA, new SKColor(255, 69, 0)},            // Orange Red for Canada to contrast with green
            {Team.CN, new SKColor(0, 0, 205)},             // Medium Blue for China
            {Team.ME, new SKColor(0, 191, 255)},           // Bright Sky Blue for Middle East
            {Team.TR, new SKColor(64, 224, 208)},          // Turquoise for Turkey
            {Team.INS, new SKColor(255, 116, 0 )},          // Bright Orange for INS
            {Team.IMF, new SKColor(0, 128, 0)},            // Green with high contrast for IMF
            {Team.WPMC, new SKColor(153, 50, 204)},        // Dark Orchid for WPMC
            {Team.Unknown, new SKColor(233, 0, 255 )},    // Hot Pink for Unknown to stand out
            {Team.GE_Wagner, new SKColor(203, 203, 203)},  // Dim Gray for GE_Wagner, still visible
            {Team.GE_UA, new SKColor(255, 223, 0)},        // Golden Yellow for GE_UA
            {Team.GE_FI, new SKColor(70, 130, 180)},       // Steel Blue for GE_FI
            {Team.GE_IS, new SKColor(75, 0, 130)},         // Indigo for GE_IS
        };
        #region Generic Extensions
        /// <summary>
        /// Restarts a timer from 0. (Timer will be started if not already running)
        /// </summary>
        public static void Restart(this System.Timers.Timer t)
        {
            t.Stop();
            t.Start();
        }

        /// <summary>
        /// Converts 'Degrees' to 'Radians'.
        /// </summary>
        public static double ToRadians(this float degrees)
        {
            return (Math.PI / 180) * degrees;
        }
        #endregion

        #region GUI Extensions
        /// <summary>
        /// Convert game position to 'Bitmap' Map Position coordinates.
        /// </summary>
        public static MapPosition ToMapPos(this System.Numerics.Vector3 vector, Map map)
        {
            return new MapPosition()
            {
                X = map.ConfigFile.X + (vector.X * map.ConfigFile.Scale),
                Y = map.ConfigFile.Y + (vector.Y * map.ConfigFile.Scale), // Invert 'Y' unity 0,0 bottom left, C# top left
                Height = vector.Z // Keep as float, calculation done later
            };
        }

        /// <summary>
        /// Gets 'Zoomed' map position coordinates.
        /// </summary>
        public static MapPosition ToZoomedPos(this MapPosition location, MapParameters mapParams)
        {
            return new MapPosition()
            {
                UIScale = mapParams.UIScale,
                X = (location.X - mapParams.Bounds.Left) * mapParams.XScale,
                Y = (location.Y - mapParams.Bounds.Top) * mapParams.YScale,
                Height = location.Height
            };
        }

        /// <summary>
        /// Gets drawing paintbrush based on Player Type
        /// </summary>
        public static SKPaint GetEntityPaint(this UActor actor) {
            if (teamEntityPaints.TryGetValue(actor.Team, out SKPaint value)) {
                return value;
            }
            SKPaint basePaint = SKPaints.PaintBase.Clone();
            basePaint.Color = TeamColors[actor.Team];
            teamEntityPaints[actor.Team] = basePaint;
            return basePaint;
        }

        /// <summary>
        /// Gets text paintbrush based on Player Type
        /// </summary>
        public static SKPaint GetTextPaint(this UActor actor)
        {
            if (teamTextPaints.TryGetValue(actor.Team, out SKPaint value)) {
                return value;
            }
            SKPaint baseText = SKPaints.TextBase.Clone();
            baseText.Color = TeamColors[actor.Team];
            teamTextPaints[actor.Team] = baseText;
            return baseText;
        }

        /// <summary>
        /// Gets projectile drawing paintbrush
        /// </summary>
        public static SKPaint GetProjectilePaint(this UActor actor) {
            if (projectilePaint != null) {
                return projectilePaint;
            }
            SKPaint basePaint = SKPaints.PaintBase.Clone();
            basePaint.Color = new SKColor(255, 0, 255);
            projectilePaint = basePaint;
            return basePaint;
        }

        /// <summary>
        /// Determines the text outline color.
        /// </summary>
        public static SKPaint GetTextOutlinePaint()
        {
            if (textOutlinePaint != null) {
                return textOutlinePaint;
            }
            SKPaint paintToUse = SKPaints.TextBaseOutline.Clone();
            paintToUse.Color = new SKColor(0, 0, 0);
            textOutlinePaint = paintToUse;
            return paintToUse;
        }
        #endregion
    }
}

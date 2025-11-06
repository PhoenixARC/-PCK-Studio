/* Copyright (c) 2025-present miku-666, MattNL
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1.The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
**/
using System.Drawing;

namespace PckStudio.Core
{
    public sealed class AtlasTile
    {
        public int Index { get; }
        public int Row { get; }
        public int Column { get; }
        public object UserData;
        private Image texture;
        public bool IsPartOfGroup => _group != null;

        public Image Texture { get => texture; set => texture = value; }

        private AtlasGroup _group;

        public AtlasTile(Image texture, int row, int column, int index, object userData)
        {
            Texture = texture;
            Row = row;
            Column = column;
            Index = index;
            UserData = userData;
        }

        internal void SetGroup(AtlasGroup group)
        {
            _group = group;
        }

        public AtlasGroup GetGroup() => _group;

        public static implicit operator Image(AtlasTile tile) => tile.Texture;

        public bool IsUserDataOfType<T>() where T : class => UserData is T _;

        public T GetUserDataOfType<T>() where T : class => UserData as T;

        public bool TryGetUserDataOfType<T>(out T value) where T : class
        {
            if (UserData is T val)
            {
                value = val;
                return true;
            }
            value = default;
            return false;
        }
    }
}
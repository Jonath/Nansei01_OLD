using UnityEngine;

public enum ETextStyle { SIMPLE, SHADOW }

public class Text : Composite {
    public Dialogue pannel;

    public Font _font;
    public int _font_size;
    public string _str;

    private float _factor;
    public ETextStyle _textstyle;

    // /!\ This is a constructor so we may want to remove it
    public Text(Font font, ETextStyle style, int font_size, string str) {
        _font = font;
        _font_size = font_size;
        _factor = 16 / _font_size;
        _str = str;
        _textstyle = style;
        Owner = EOwner.NONE;
    }

    public override void SetupUVs(Vector2[] _uvs, Texture tex) { // Texture unused
        // Regenerate font texture on the fly
        _font.RequestCharactersInTexture(_str, _font_size);

        for (int i = 0; i < _nb_composites; ++i) {
            CharacterInfo ch;
            _font.GetCharacterInfo(_str[i], out ch);

            int uv_idx = _composites[i].Index * 4;
            _uvs[uv_idx] = ch.uvTopLeft;
            _uvs[uv_idx + 1] = ch.uvTopRight;
            _uvs[uv_idx + 2] = ch.uvBottomRight;
            _uvs[uv_idx + 3] = ch.uvBottomLeft;
        }
    }

    public override void SetupVertices(Vector3[] _vertices, Color32[] _colors) {
        Vector3 pos = Position;
        float ch_height = 0;

        int word_idx = 0;
        int jump_idx = 1;
        float current_line_width = 0;

        string[] words = new string[0];
        if (_str != null) { 
            words = _str.Split(' ');
        }

        float pannel_width = pannel.obj.Width;

        for (int i = 0; i < _nb_composites; ++i) {
            CharacterInfo ch;
            _font.GetCharacterInfo(_str[i], out ch);

            // Only store the first character height
            if(i == 0) ch_height = ch.glyphHeight;

            int vert_idx = _composites[i].Index * 4;
            _vertices[vert_idx] = pos + new Vector3(ch.minX, ch.maxY - ch_height) * _factor;
            _vertices[vert_idx + 1] = pos + new Vector3(ch.maxX, ch.maxY - ch_height) * _factor;
            _vertices[vert_idx + 2] = pos + new Vector3(ch.maxX, ch.minY - ch_height) * _factor;
            _vertices[vert_idx + 3] = pos + new Vector3(ch.minX, ch.minY - ch_height) * _factor;

            _colors[vert_idx] = _colors[vert_idx + 1] = _colors[vert_idx + 2] = _colors[vert_idx + 3] = Color;
            pos += new Vector3(ch.advance * _factor, 0);

            // Can the next word fit on this line ?
            if(i == 0 || _str[i] == ' ') {
                float word_width = ComputeWordWidth(words[word_idx]);
                current_line_width += word_width;
                if (current_line_width > pannel_width - pannel.margin) {
                    pos = new Vector3(Position.x, Position.y - pannel.linejump * jump_idx);
                    current_line_width = word_width;
                    jump_idx++;
                }
                word_idx++;
            }
        }
    }

    private float ComputeWordWidth(string word) {
        float word_width = 0;
        for (int i = 0; i < word.Length; ++i) {
            CharacterInfo ch;
            _font.GetCharacterInfo(word[i], out ch);
            word_width += (ch.maxX - ch.minX) * _factor;
        }
        return word_width;
    }

    public override void SetupTriangles(int[] _indices) {
        for (int i = 0; i < _nb_composites; ++i)
        {
            int tri_idx = _composites[i].Index * 6;
            int vert_idx = _composites[i].Index * 4;

            _indices[tri_idx] = vert_idx;
            _indices[tri_idx + 1] = vert_idx + 1;
            _indices[tri_idx + 2] = vert_idx + 2;
            _indices[tri_idx + 3] = vert_idx + 0;
            _indices[tri_idx + 4] = vert_idx + 2;
            _indices[tri_idx + 5] = vert_idx + 3;
        }
    }

    public override void ComputePosition(Vector3[] _vertices, Color32[] _colors, float dt = 0) {
        SetupVertices(_vertices, _colors);
    }
}
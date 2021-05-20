using System;

namespace Atheneum.Dto.Robo
{
    /// <summary>
    /// Мета данные о роботе
    /// </summary>
    public class RoboDto
    {
        /// <summary>
        /// Идентификатор робота, выдаётся при регистрации
        /// </summary>
        public Guid? Id { get; set; } = Guid.Empty;

        /// <summary>
        /// Наименование робота
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Краткое описание робота
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Идентификатор хозяина
        /// </summary>
        public Guid? MasterId { get; set; }
    }
}

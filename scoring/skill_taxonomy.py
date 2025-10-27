"""Skill taxonomy for hierarchical matching"""

SKILL_TAXONOMY = {
    'python': ['pandas', 'numpy', 'scikit-learn', 'django', 'flask', 'fastapi'],
    'machine learning': ['deep learning', 'neural networks', 'tensorflow', 'pytorch', 'keras'],
    'data science': ['machine learning', 'statistics', 'data analysis', 'visualization'],
    'cloud': ['aws', 'azure', 'gcp', 'google cloud'],
    'devops': ['docker', 'kubernetes', 'ci/cd', 'jenkins', 'terraform'],
    'web development': ['react', 'angular', 'vue.js', 'node.js', 'javascript'],
    'backend': ['api', 'rest', 'graphql', 'microservices', 'database'],
    'frontend': ['html', 'css', 'javascript', 'react', 'responsive design'],
    'database': ['sql', 'mysql', 'postgresql', 'mongodb', 'redis'],
    'testing': ['unit testing', 'integration testing', 'pytest', 'jest', 'selenium']
}

def get_related_skills(skill: str) -> list:
    """Get related skills from taxonomy"""
    skill_lower = skill.lower()
    
    # Check if skill is a parent
    if skill_lower in SKILL_TAXONOMY:
        return SKILL_TAXONOMY[skill_lower]
    
    # Check if skill is a child
    for parent, children in SKILL_TAXONOMY.items():
        if skill_lower in children:
            return [parent] + children
    
    return []
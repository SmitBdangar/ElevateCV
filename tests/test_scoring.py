"""Tests for scoring engine"""
import pytest
from scoring.enhanced_scoring import EnhancedScoringEngine

def test_skill_matching():
    """Test skill match calculation"""
    scorer = EnhancedScoringEngine(use_transformers=False)
    
    resume_skills = ['python', 'tensorflow', 'aws']
    required_skills = ['python', 'machine learning', 'aws']
    
    score, details = scorer.calculate_skill_match(
        resume_skills,
        required_skills
    )
    
    assert score > 0
    assert score <= 100
    assert 'direct_matches' in details

if __name__ == "__main__":
    pytest.main([__file__, "-v"])